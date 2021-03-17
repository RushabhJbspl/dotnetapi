using Worldex.Core.ApiModels;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities;
using Worldex.Core.Entities.Communication;
using Worldex.Core.Entities.Configuration.FeedConfiguration;
using Worldex.Core.Entities.Transaction;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.FeedConfiguration;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.Services.RadisDatabase;
using Worldex.Core.SignalR;
using Worldex.Core.ViewModels.Transaction;
using Worldex.Core.ViewModels.Transaction.Arbitrage;
using Worldex.Core.ViewModels.Wallet;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.Data;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Worldex.Infrastructure.Services
{
    public class SignalRService : ISignalRService
    {
        private readonly ISignalRQueue _signalRQueue;
        private readonly ILogger<SignalRService> _logger;
        private readonly IMediator _mediator;
        private readonly IFrontTrnRepository _frontTrnRepository;
        private RedisConnectionFactory _fact;
        public String Token = null;
        public string ControllerName = "SignalRService";
        private SocketHub _chat;
        private ThirdPartySocketHub _clientHub; // khushali 12-03-2019 for Client Socket API access 
        private readonly IConfiguration _configuration;
        private readonly IExchangeFeedConfiguration _exchangeFeedConfiguration;
        private ICommonRepository<UserAPIKeyDetails> _userAPIKeyDetailsRepository;
        private readonly ThirdPartyAPISignalRQueue _thirdPartyAPISignalRQueue;
        private readonly ITrnMasterConfiguration _ITrnMasterConfiguration;

        public SignalRService(ILogger<SignalRService> logger, IMediator mediator, IFrontTrnRepository frontTrnRepository,
             RedisConnectionFactory Factory, ISignalRQueue signalRQueue, SocketHub chat, IConfiguration Configuration,
             IExchangeFeedConfiguration exchangeFeedConfiguration, ICommonRepository<UserAPIKeyDetails> UserAPIKeyDetailsRepository,
             ThirdPartySocketHub ClientHub, ThirdPartyAPISignalRQueue thirdPartyAPISignalRQueue, ITrnMasterConfiguration ITrnMasterConfiguration)
        {
            _fact = Factory;
            _logger = logger;
            _mediator = mediator;
            // _TransactionRepository = TransactionRepository;
            _frontTrnRepository = frontTrnRepository;
            //_TradeTransactionRepository = TradeTransactionRepository;
            _signalRQueue = signalRQueue;
            _chat = chat;
            _configuration = Configuration;
            _exchangeFeedConfiguration = exchangeFeedConfiguration;
            _userAPIKeyDetailsRepository = UserAPIKeyDetailsRepository;
            _clientHub = ClientHub; // khushali 12-03-2019 for Client Socket API access 
            _thirdPartyAPISignalRQueue = thirdPartyAPISignalRQueue;
            _ITrnMasterConfiguration = ITrnMasterConfiguration;
        }

        #region Pairwise Reguler Method

        public async Task BuyerBook(GetBuySellBook Data, string Pair, string UserID, short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                SignalRComm<GetBuySellBook> CommonData = new SignalRComm<GetBuySellBook>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.BuyerBook);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveBuyerBook);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;
                CommonData.LP = 0;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.BuyerBook;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                _chat.BuyerBook(SendData.Parameter, SendData.DataObj);

                ThirdPartyAPISinalR TPSendData = new ThirdPartyAPISinalR();
                TPSendData.Method = enMethodName.BuyerBook;
                TPSendData.DataObj = JsonConvert.SerializeObject(CommonData);
                TPSendData.Parameter = CommonData.Parameter;
                TPSendData.UserID = UserID;
                TPSendData.IsPrivate = 0;
                _thirdPartyAPISignalRQueue.Enqueue(TPSendData);
                
                //var res = _exchangeFeedConfiguration.CheckFeedDataLimit(System.Text.ASCIIEncoding.ASCII.GetByteCount(SendData.DataObj), Convert.ToInt16(SendData.Method));
                //if(res.ErrorCode==enErrorCode.Success)
                //Task UserHub = _chat.BuyerBook(SendData.Parameter, SendData.DataObj);
                //Task ClientHub = _clientHub.BuyerBook(SendData.Parameter, SendData.DataObj);
                //Task.WaitAll();
                //Task.Run(() => _signalRQueue.Enqueue(SendData));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task SellerBook(GetBuySellBook Data, string Pair, string UserID, short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                SignalRComm<GetBuySellBook> CommonData = new SignalRComm<GetBuySellBook>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.SellerBook);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveSellerBook);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;
                CommonData.LP = 0;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.SellerBook;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                Task UserHub = _chat.SellerBook(SendData.Parameter, SendData.DataObj);

                ThirdPartyAPISinalR TPSendData = new ThirdPartyAPISinalR();
                TPSendData.Method = enMethodName.SellerBook;
                TPSendData.DataObj = JsonConvert.SerializeObject(CommonData);
                TPSendData.Parameter = CommonData.Parameter;
                TPSendData.UserID = UserID;
                TPSendData.IsPrivate = 0;
                _thirdPartyAPISignalRQueue.Enqueue(TPSendData);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task OrderHistory(GetOrderHistoryInfo Data, string Pair, string UserID, short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                SignalRComm<GetOrderHistoryInfo> CommonData = new SignalRComm<GetOrderHistoryInfo>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.OrderHistory);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveOrderHistory);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.OrderHistory;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                Task UserHub = _chat.OrderHistory(SendData.Parameter, SendData.DataObj);

                ThirdPartyAPISinalR TPSendData = new ThirdPartyAPISinalR();
                TPSendData.Method = enMethodName.OrderHistory;
                TPSendData.DataObj = JsonConvert.SerializeObject(CommonData);
                TPSendData.Parameter = CommonData.Parameter;
                TPSendData.UserID = UserID;
                TPSendData.IsPrivate = 0;
                _thirdPartyAPISignalRQueue.Enqueue(TPSendData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }
        public async Task ChartData(GetGraphDetailInfo Data, string Pair, short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                SignalRComm<GetGraphDetailInfo> CommonData = new SignalRComm<GetGraphDetailInfo>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.ChartData);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveChartData);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.ChartData;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                Task UserHub = _chat.ChartData(SendData.Parameter, SendData.DataObj);
                Task ClientHub = _clientHub.ChartData(SendData.Parameter, SendData.DataObj);
                Task.WaitAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task ChartDataEveryLastMin(DateTime DateTime, short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                List<GetGraphResponsePairWise> GraphResponsesList = new List<GetGraphResponsePairWise>();
                if (IsMargin == 1)
                    GraphResponsesList = _frontTrnRepository.GetGraphDataEveryLastMinMargin(DateTime.ToString("yyyy-MM-dd HH:mm:00:000"));
                else
                    GraphResponsesList = _frontTrnRepository.GetGraphDataEveryLastMin(DateTime.ToString("yyyy-MM-dd HH:mm:00:000"));

                if (GraphResponsesList != null)  // Uday 01-03-2019   Handle null response
                {
                    foreach (GetGraphResponsePairWise GraphData in GraphResponsesList)
                    {
                        GetGraphDetailInfo GraphDetailInfo = new GetGraphDetailInfo();
                        GraphDetailInfo.Close = GraphData.CloseVal;
                        GraphDetailInfo.High = GraphData.High;
                        GraphDetailInfo.Open = GraphData.OpenVal;
                        GraphDetailInfo.Low = GraphData.Low;
                        DateTime dt2 = new DateTime(1970, 1, 1);
                        GraphDetailInfo.DataDate = GraphData.DataDate;
                        GraphDetailInfo.Volume = GraphData.Volume;
                        GraphDetailInfo.Close = GraphData.CloseVal;
                        ChartData(GraphDetailInfo, GraphData.PairName, IsMargin);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
               
            }
        }

        public async Task MarketData(MarketCapData Data, string Pair, string UserID, short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                SignalRComm<MarketCapData> CommonData = new SignalRComm<MarketCapData>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.MarketData);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveMarketData);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.MarketData;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                Task UserHub = _chat.MarketData(SendData.Parameter, SendData.DataObj);

                ThirdPartyAPISinalR TPSendData = new ThirdPartyAPISinalR();
                TPSendData.Method = enMethodName.MarketData;
                TPSendData.DataObj = JsonConvert.SerializeObject(CommonData);
                TPSendData.Parameter = CommonData.Parameter;
                TPSendData.UserID = UserID;
                TPSendData.IsPrivate = 0;
                _thirdPartyAPISignalRQueue.Enqueue(TPSendData);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
               
            }
        }

        public async Task LastPrice(LastPriceViewModel Data, string Pair, string UserID, short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                SignalRComm<LastPriceViewModel> CommonData = new SignalRComm<LastPriceViewModel>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.Price);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveLastPrice);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.Price;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                Task UserHub = _chat.LastPrice(SendData.Parameter, SendData.DataObj);

                ThirdPartyAPISinalR TPSendData = new ThirdPartyAPISinalR();
                TPSendData.Method = enMethodName.Price;
                TPSendData.DataObj = JsonConvert.SerializeObject(CommonData);
                TPSendData.Parameter = CommonData.Parameter;
                TPSendData.UserID = UserID;
                TPSendData.IsPrivate = 0;
                _thirdPartyAPISignalRQueue.Enqueue(TPSendData);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task StopLimitBuyerBook(List<StopLimitBuySellBook> Data, string Pair, string UserID, short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                SignalRComm<List<StopLimitBuySellBook>> CommonData = new SignalRComm<List<StopLimitBuySellBook>>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.StopLimitBuyerBook);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveStopLimitBuyerBook);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.StopLimitBuyerBook;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                Task UserHub = _chat.StopLimitBuyerBook(SendData.Parameter, SendData.DataObj);

                ThirdPartyAPISinalR TPSendData = new ThirdPartyAPISinalR();
                TPSendData.Method = enMethodName.StopLimitBuyerBook;
                TPSendData.DataObj = JsonConvert.SerializeObject(CommonData);
                TPSendData.Parameter = CommonData.Parameter;
                TPSendData.UserID = UserID;
                TPSendData.IsPrivate = 0;
                _thirdPartyAPISignalRQueue.Enqueue(TPSendData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task StopLimitSellerBook(List<StopLimitBuySellBook> Data, string Pair, string UserID, short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                SignalRComm<List<StopLimitBuySellBook>> CommonData = new SignalRComm<List<StopLimitBuySellBook>>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.StopLimitSellerBook);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveStopLimitSellerBook);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;
                CommonData.IsMargin = IsMargin;


                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.StopLimitSellerBook;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                Task UserHub = _chat.StopLimitSellerBook(SendData.Parameter, SendData.DataObj);
                Task ClientHub = _clientHub.StopLimitSellerBook(SendData.Parameter, SendData.DataObj);

                ThirdPartyAPISinalR TPSendData = new ThirdPartyAPISinalR();
                TPSendData.Method = enMethodName.StopLimitSellerBook;
                TPSendData.DataObj = JsonConvert.SerializeObject(CommonData);
                TPSendData.Parameter = CommonData.Parameter;
                TPSendData.UserID = UserID;
                TPSendData.IsPrivate = 0;
                _thirdPartyAPISignalRQueue.Enqueue(TPSendData);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
               
            }
        }

        public async Task BulkBuyerBook(List<GetBuySellBook> Data, string Pair, enLiquidityProvider LP, short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                SignalRComm<List<GetBuySellBook>> CommonData = new SignalRComm<List<GetBuySellBook>>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.BulkBuyerBook);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.ReceiveBulkBuyerBook);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;
                CommonData.LP = Convert.ToInt16(LP);
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.BulkBuyerBook;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                await _chat.BuyerBookLP(SendData.Parameter, SendData.DataObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task BulkSellerBook(List<GetBuySellBook> Data, string Pair, enLiquidityProvider LP, short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                SignalRComm<List<GetBuySellBook>> CommonData = new SignalRComm<List<GetBuySellBook>>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.BulkSellerBook);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.ReceiveBulkSellerBook);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;
                CommonData.LP = Convert.ToInt16(LP);
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.BulkSellerBook;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                await _chat.SellerBookLP(SendData.Parameter, SendData.DataObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task BulkOrderHistory(List<GetTradeHistoryInfoV1> Data, string Pair, enLiquidityProvider LP, short IsMargin = 0)
        {
            try
            {
                SignalRComm<List<GetTradeHistoryInfoV1>> CommonData = new SignalRComm<List<GetTradeHistoryInfoV1>>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.BulkOrderHistory);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.ReceiveBulkOrderHistory);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;
                CommonData.LP = Convert.ToInt16(LP);
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.BulkSellerBook;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                await _chat.OrderHistoryLP(SendData.Parameter, SendData.DataObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                
            }
        }

        #endregion

        #region UserSpecific
        // khushali 12-03-2019 Add UserID argument for all user specific method
        public async Task ActiveOrder(ActiveOrderInfoV1 Data, string Token, string UserID, short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                SignalRComm<ActiveOrderInfoV1> CommonData = new SignalRComm<ActiveOrderInfoV1>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.ActiveOrder);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveActiveOrder);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                CommonData.Data = Data;
                CommonData.Parameter = null;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.ActiveOrder;
                SendData.Parameter = Token;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                var UserHub = _chat.ActiveOrder(SendData.Parameter, SendData.DataObj);

                ThirdPartyAPISinalR TPSendData = new ThirdPartyAPISinalR();
                TPSendData.Method = enMethodName.ActiveOrder;
                TPSendData.DataObj = JsonConvert.SerializeObject(CommonData);
                TPSendData.UserID = UserID;
                TPSendData.IsPrivate = 1;
                _thirdPartyAPISignalRQueue.Enqueue(TPSendData);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                
            }
        }

        public async Task OpenOrder(OpenOrderInfo Data, string Token, string UserID, short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                SignalRComm<OpenOrderInfo> CommonData = new SignalRComm<OpenOrderInfo>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.OpenOrder);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveOpenOrder);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                CommonData.Data = Data;
                CommonData.Parameter = null;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.OpenOrder;
                SendData.Parameter = Token;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                var UserHub = _chat.OpenOrder(SendData.Parameter, SendData.DataObj);

                ThirdPartyAPISinalR TPSendData = new ThirdPartyAPISinalR();
                TPSendData.Method = enMethodName.OpenOrder;
                TPSendData.DataObj = JsonConvert.SerializeObject(CommonData);
                TPSendData.UserID = UserID;
                TPSendData.IsPrivate = 1;
                _thirdPartyAPISignalRQueue.Enqueue(TPSendData);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task TradeHistory(GetTradeHistoryInfoV1 Data, string Token, string UserID, short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                SignalRComm<GetTradeHistoryInfoV1> CommonData = new SignalRComm<GetTradeHistoryInfoV1>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.TradeHistory);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveTradeHistory);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                CommonData.Data = Data;
                CommonData.Parameter = null;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.TradeHistory;
                SendData.Parameter = Token;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                var UserHub = _chat.TradeHistory(SendData.Parameter, SendData.DataObj);

                ThirdPartyAPISinalR TPSendData = new ThirdPartyAPISinalR();
                TPSendData.Method = enMethodName.TradeHistory;
                TPSendData.DataObj = JsonConvert.SerializeObject(CommonData);
                TPSendData.UserID = UserID;
                TPSendData.IsPrivate = 1;
                _thirdPartyAPISignalRQueue.Enqueue(TPSendData);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                
            }
        }

        public async Task RecentOrder(RecentOrderInfoV1 Data, string Token, string UserID, short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                SignalRComm<RecentOrderInfoV1> CommonData = new SignalRComm<RecentOrderInfoV1>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.RecentOrder);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveRecentOrder);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                CommonData.Data = Data;
                CommonData.Parameter = null;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.RecentOrder;
                SendData.Parameter = Token;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                ThirdPartyAPISinalR TPSendData = new ThirdPartyAPISinalR();
                TPSendData.Method = enMethodName.RecentOrder;
                TPSendData.DataObj = JsonConvert.SerializeObject(CommonData);
                TPSendData.UserID = UserID;
                TPSendData.IsPrivate = 1;
                _thirdPartyAPISignalRQueue.Enqueue(TPSendData);
                var UserHub = _chat.RecentOrder(SendData.Parameter, SendData.DataObj);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
               
            }
        }

        public async Task WalletBalUpdate(WalletMasterResponse Data, string Wallet, string Token, string UserID, short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                SignalRComm<WalletMasterResponse> CommonData = new SignalRComm<WalletMasterResponse>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.BuyerSideWallet);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveWalletBal);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                CommonData.Data = Data;
                CommonData.Parameter = null;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.BuyerSideWallet;
                SendData.Parameter = Token;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                SendData.WalletName = Wallet;
                var UserHub = _chat.WalletBalUpdate(SendData.Parameter, SendData.WalletName, SendData.DataObj);

                ThirdPartyAPISinalR TPSendData = new ThirdPartyAPISinalR();
                TPSendData.Method = enMethodName.BuyerSideWallet;
                TPSendData.DataObj = JsonConvert.SerializeObject(CommonData);
                TPSendData.UserID = UserID;
                TPSendData.WalletName = Wallet;
                TPSendData.IsPrivate = 1;
                _thirdPartyAPISignalRQueue.Enqueue(TPSendData);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task ActivityNotificationV2(ActivityNotificationMessage Notification, string Token, string UserID, short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                SignalRComm<ActivityNotificationMessage> CommonData = new SignalRComm<ActivityNotificationMessage>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.ActivityNotification);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveNotification);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                CommonData.Data = Notification;
                CommonData.Parameter = null;
                CommonData.IsMargin = IsMargin;

                //SignalRDataNotify SendData = new SignalRDataNotify();
                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.ActivityNotification;
                SendData.Parameter = Token;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                var UserHub = _chat.ActivityNotification(SendData.Parameter, SendData.DataObj);

                ThirdPartyAPISinalR TPSendData = new ThirdPartyAPISinalR();
                TPSendData.Method = enMethodName.ActivityNotification;
                TPSendData.DataObj = JsonConvert.SerializeObject(CommonData);
                TPSendData.UserID = UserID;
                TPSendData.IsPrivate = 1;
                _thirdPartyAPISignalRQueue.Enqueue(TPSendData);
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        // khushali 12-01-2019
        public async Task ActivityList(ListAddWalletRequest Request, string Token, string UserID, short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                SignalRComm<ListAddWalletRequest> CommonData = new SignalRComm<ListAddWalletRequest>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.WalletActivity);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveWalletActivity);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                CommonData.Data = Request;
                CommonData.Parameter = null;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.WalletActivity;
                SendData.Parameter = Token;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                var UserHub = _chat.WalletActivity(SendData.Parameter, SendData.DataObj);

                ThirdPartyAPISinalR TPSendData = new ThirdPartyAPISinalR();
                TPSendData.Method = enMethodName.WalletActivity;
                TPSendData.DataObj = JsonConvert.SerializeObject(CommonData);
                TPSendData.UserID = UserID;
                TPSendData.IsPrivate = 1;
                _thirdPartyAPISignalRQueue.Enqueue(TPSendData);
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        #endregion

        #region BaseMarket
        public async Task PairData(VolumeDataRespose Data, string Base, string UserID, short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                SignalRComm<VolumeDataRespose> CommonData = new SignalRComm<VolumeDataRespose>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.PairData);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecievePairData);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.Base);
                CommonData.Data = Data;
                CommonData.Parameter = Base;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.PairData;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                Task UserHub = _chat.PairData(SendData.Parameter, SendData.DataObj);

                ThirdPartyAPISinalR TPSendData = new ThirdPartyAPISinalR();
                TPSendData.Method = enMethodName.PairData;
                TPSendData.DataObj = JsonConvert.SerializeObject(CommonData);
                TPSendData.Parameter = CommonData.Parameter;
                TPSendData.UserID = UserID;
                TPSendData.IsPrivate = 0;
                _thirdPartyAPISignalRQueue.Enqueue(TPSendData);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
              
            }
        }

        public async Task MarketTicker(List<VolumeDataRespose> Data, string UserID, string Base = "", short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                SignalRComm<List<VolumeDataRespose>> CommonData = new SignalRComm<List<VolumeDataRespose>>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.MarketTicker);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveMarketTicker);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.Base);
                CommonData.Data = Data;
                CommonData.Parameter = Base;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.MarketTicker;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                Task UserHub = _chat.MarketTicker(SendData.Parameter, SendData.DataObj);

                ThirdPartyAPISinalR TPSendData = new ThirdPartyAPISinalR();
                TPSendData.Method = enMethodName.MarketTicker;
                TPSendData.DataObj = JsonConvert.SerializeObject(CommonData);
                TPSendData.Parameter = CommonData.Parameter;
                TPSendData.UserID = UserID;
                TPSendData.IsPrivate = 0;
                _thirdPartyAPISignalRQueue.Enqueue(TPSendData);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                
            }
        }
        #endregion

        #region GlobalEvents
        public async Task SendOrderHistory(GetOrderHistoryInfo historyInfo,String PairName, string UserID ,short IsMargin = 0)
        {
            try
            {
                OrderHistory(historyInfo, PairName, UserID, IsMargin);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }

        public async Task OnStatusSuccess(short Status, TransactionQueue Newtransaction, TradeTransactionQueue NewTradeTransaction, string Token, short OrderType, decimal SettlementPrice)
        {
            //update Recent Order
            //pop OpenOrder
            //add tradehistory
            //add orderhistory
            //pop buyer/seller book;
            //DateTime curtime = DateTime.UtcNow;
            string UserID = NewTradeTransaction.MemberID.ToString();
            try
            {
                GetBuySellBook BuySellmodel = new GetBuySellBook();

                Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusSuccess" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "0 start Socket call       TRNNO : " + Newtransaction.Id));
                if (string.IsNullOrEmpty(Token))
                {
                    Token = GetTokenByUserID(NewTradeTransaction.MemberID.ToString());
                }
                if (!string.IsNullOrEmpty(Token))
                {
                    List<GetBuySellBook> list = new List<GetBuySellBook>();
                    if (NewTradeTransaction.TrnType == 4)//Buy
                    {
                        list = _frontTrnRepository.GetBuyerBook(NewTradeTransaction.PairID, NewTradeTransaction.BidPrice);
                        foreach (var model in list)
                        {
                            BuySellmodel = model;
                            break;
                        }
                        if (BuySellmodel.OrderId.ToString() != "00000000-0000-0000-0000-000000000000")
                        {

                            Parallel.Invoke(() => BuyerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString()),
                                            () => HelperForLog.WriteLogForSocket("OnStatusSuccess" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 BuyerBook Update call          TRNNO : " + Newtransaction.Id));
                        }
                        else
                        {
                            BuySellmodel.Amount = 0;
                            BuySellmodel.OrderId = new Guid();
                            BuySellmodel.RecordCount = 0;
                            BuySellmodel.Price = NewTradeTransaction.BidPrice;
                            Task.Run(() => Parallel.Invoke(() => BuyerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString()),
                                () => HelperForLog.WriteLogForSocket("OnStatusSuccess" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 BuyerBook pop call       TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName + " : No Data Found")));
                        }

                    }
                    else//Sell
                    {
                        list = _frontTrnRepository.GetSellerBook(NewTradeTransaction.PairID, NewTradeTransaction.AskPrice);
                        foreach (var model in list)
                        {

                            BuySellmodel = model;
                            break;
                        }
                        if (BuySellmodel.OrderId.ToString() != "00000000-0000-0000-0000-000000000000")
                        {
                            Parallel.Invoke(() => SellerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString()),
                                            () => HelperForLog.WriteLogForSocket("OnStatusSuccess" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 SellerBook Update call         TRNNO : " + Newtransaction.Id));
                        }
                        else
                        {
                            BuySellmodel.Amount = 0;
                            BuySellmodel.OrderId = new Guid();
                            BuySellmodel.RecordCount = 0;
                            BuySellmodel.Price = NewTradeTransaction.AskPrice;
                            Task.Run(() => Parallel.Invoke(() => SellerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString()),
                                () => HelperForLog.WriteLogForSocket("OnStatusSuccess" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 SellerBook pop call          TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName + " : No Data Found")));
                        }

                    }

                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                    notification.MsgCode = Convert.ToInt32(enErrorCode.SignalRTrnSuccessfullySettled);
                    var Price = (NewTradeTransaction.TrnType == 4) ? (SettlementPrice == 0 ? NewTradeTransaction.BidPrice : SettlementPrice) : NewTradeTransaction.AskPrice;
                    var Amount = (NewTradeTransaction.TrnType == 4) ? NewTradeTransaction.BuyQty : NewTradeTransaction.SellQty;
                    var Charge =Convert.ToDecimal(Newtransaction.ChargeRs == null ? 0 : Newtransaction.ChargeRs);
                    notification.MsgCode = Convert.ToInt32(enErrorCode.SignalRTrnSuccessfullySettled);
                    notification.Param1 = Price.ToString();
                    notification.Param2 = Amount.ToString();
                    notification.Param3 = Convert.ToString(NewTradeTransaction.TrnTypeName == "BUY" ? ((Price * Amount) - Charge) : ((Price * Amount)));
                    notification.Type = Convert.ToInt16(EnNotificationType.Success);

                    if (OrderType == 3)
                    {
                        Task.Run(() => Parallel.Invoke(() => GetAndSendRecentOrderData(Newtransaction, NewTradeTransaction, Token, OrderType, UserID).Wait(),
                                   // () => GetAndSendTradeHistoryInfoData(Newtransaction, NewTradeTransaction, Token,UserID,SettlementPrice: SettlementPrice),
                                    () => ActivityNotificationV2(notification, Token, UserID)));
                        GetAndSendTradeHistoryInfoData(Newtransaction, NewTradeTransaction, Token, UserID, SettlementPrice: SettlementPrice);
                    }
                    else
                    {
                        Task.Run(() => Parallel.Invoke(() => GetAndSendRecentOrderData(Newtransaction, NewTradeTransaction, Token, OrderType, UserID).Wait(),
                                   () => GetAndSendActiveOrderData(Newtransaction, NewTradeTransaction, Token, OrderType, UserID, 1),
                                   //() => GetAndSendTradeHistoryInfoData(Newtransaction, NewTradeTransaction, Token, UserID, SettlementPrice: SettlementPrice),
                                   () => ActivityNotificationV2(notification, Token, UserID)));
                        GetAndSendTradeHistoryInfoData(Newtransaction, NewTradeTransaction, Token, UserID, SettlementPrice: SettlementPrice);
                    }
                    Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusSuccess" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "2 Complete Socket call    TRNNO : " + Newtransaction.Id));

                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }

        public async Task OnStatusPartialSuccess(short Status, TransactionQueue Newtransaction, TradeTransactionQueue NewTradeTransaction, string Token, short OrderType)
        {
            //update Buyer/seller book
            string UserID = NewTradeTransaction.MemberID.ToString();
            try
            {
                GetBuySellBook BuySellmodel = new GetBuySellBook();
                if (string.IsNullOrEmpty(Token))
                {
                    Token = GetTokenByUserID(NewTradeTransaction.MemberID.ToString());
                }
                if (!string.IsNullOrEmpty(Token))
                {
                    List<GetBuySellBook> list = new List<GetBuySellBook>();
                    if (NewTradeTransaction.TrnType == 4)//Buy
                    {
                        list =_frontTrnRepository.GetBuyerBook(NewTradeTransaction.PairID, NewTradeTransaction.BidPrice);
                        foreach (var model in list)
                        {
                            BuySellmodel = model;
                            break;
                        }
                        if (BuySellmodel.OrderId.ToString() != "00000000-0000-0000-0000-000000000000")
                        {

                            Parallel.Invoke(() => BuyerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString()),
                                            () => HelperForLog.WriteLogForSocket("OnStatusPartialSuccess" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 BuyerBook call          TRNNO : " + Newtransaction.Id));
                        }
                        else
                            Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusPartialSuccess" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 BuyerBook call          TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName + " : No Data Found"));
                    }
                    else//Sell
                    {
                        list = _frontTrnRepository.GetSellerBook(NewTradeTransaction.PairID, NewTradeTransaction.AskPrice);
                        foreach (var model in list)
                        {

                            BuySellmodel = model;
                            break;
                        }
                        if (BuySellmodel.OrderId.ToString() != "00000000-0000-0000-0000-000000000000")
                        {
                            Parallel.Invoke(() => SellerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString()),
                                            () => HelperForLog.WriteLogForSocket("OnStatusPartialSuccess" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 SellerBook call         TRNNO : " + Newtransaction.Id));
                        }
                        else
                            Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusPartialSuccess" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 SellerBook call         TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName + " : No Data Found"));
                    }
                    //Rita 13-3-19 for Settled Qty update
                    if (OrderType != 3)//for market ordre not sent open and recent ordre 
                    {

                        Task.Run(() => Parallel.Invoke(() => GetAndSendActiveOrderData(Newtransaction, NewTradeTransaction, Token, OrderType, UserID),
                                   () => GetAndSendRecentOrderData(Newtransaction, NewTradeTransaction, Token, OrderType, UserID)));

                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
               
            }
        }

        public async Task OnStatusHold(short Status, TransactionQueue Newtransaction, TradeTransactionQueue NewTradeTransaction, string Token, short OrderType)
        {
            //add buyer/seller book
            //add OpenOrder
            //add recent order
            string UserID = NewTradeTransaction.MemberID.ToString();
            try
            {
                GetBuySellBook BuySellmodel = new GetBuySellBook();
                Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusHold" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "0 start Socket call       TRNNO : " + Newtransaction.Id));
                if (string.IsNullOrEmpty(Token))
                {
                    Token = GetTokenByUserID(NewTradeTransaction.MemberID.ToString());
                }
                List<GetBuySellBook> list = new List<GetBuySellBook>();
                if (!string.IsNullOrEmpty(Token))
                {
                    if (OrderType == 4)
                    {
                        HelperForLog.WriteLogForSocket("OnStatusHold" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, " Order type 4 call OnLtpVhange    TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName);
                        this.OnLtpChange(0, NewTradeTransaction.PairID, NewTradeTransaction.PairName, UserID: NewTradeTransaction.MemberID.ToString(), IsCancel: 1);
                    }
                    else
                    {
                        if (NewTradeTransaction.TrnType == 4)//Buy
                        {
                            list = _frontTrnRepository.GetBuyerBook(NewTradeTransaction.PairID, NewTradeTransaction.BidPrice);
                            foreach (var model in list)
                            {
                                //BuySellmodel = model;
                                BuySellmodel.Amount = model.Amount;
                                BuySellmodel.Price = model.Price;
                                BuySellmodel.OrderId = model.OrderId;
                                BuySellmodel.RecordCount = model.RecordCount;
                                break;
                            }
                            if (BuySellmodel.OrderId.ToString() != "00000000-0000-0000-0000-000000000000")
                            {
                                Parallel.Invoke(() => BuyerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString()),
                                                () => HelperForLog.WriteLogForSocket("OnStatusHold" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 BuyerBook call          TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName + " BuyerBook Amount " + BuySellmodel.Amount + " Price " + BuySellmodel.Price));
                            }
                            else
                                Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusHold" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 BuyerBook call          TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName + " : No Data Found"));

                        }
                        else//Sell
                        {
                            list = _frontTrnRepository.GetSellerBook(NewTradeTransaction.PairID, NewTradeTransaction.AskPrice);
                            foreach (var model in list)
                            {
                                //BuySellmodel = model;
                                BuySellmodel.Price = model.Price;
                                BuySellmodel.Amount = model.Amount;
                                BuySellmodel.OrderId = model.OrderId;
                                BuySellmodel.RecordCount = model.RecordCount;
                                break;
                            }
                            if (BuySellmodel.OrderId.ToString() != "00000000-0000-0000-0000-000000000000")
                            {
                                Parallel.Invoke(() => SellerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString()),
                                                () => HelperForLog.WriteLogForSocket("OnStatusHold" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 SellerBook call         TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName + " SellerBook Amount " + BuySellmodel.Amount + " Price " + BuySellmodel.Price));
                            }
                            else
                                Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusHold" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 SellerBook call         TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName + " : No Data Found"));

                        }
                    }
                    if (OrderType != 3)//for market ordre not sent open and recent ordre 
                    {
                        Task.Run(() => Parallel.Invoke(() => GetAndSendActiveOrderData(Newtransaction, NewTradeTransaction, Token, OrderType, UserID),
                           () => GetAndSendRecentOrderData(Newtransaction, NewTradeTransaction, Token, OrderType, UserID)));
                    }

                    Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusHold" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "2 Complete Socket call    TRNNO : " + Newtransaction.Id));
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }

        public async Task OnStatusCancel(short Status, TransactionQueue Newtransaction, TradeTransactionQueue NewTradeTransaction, string Token, short OrderType, short IsPartialCancel = 0, int OrderCount = 1)
        {
            //pop from OpenOrder
            //update Recent order
            //Buyer/Seller pop
            string UserID = NewTradeTransaction.MemberID.ToString();
            try
            {
                GetBuySellBook BuySellmodel = new GetBuySellBook();
                Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusCancel " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "0 start Socket call       TRNNO : " + Newtransaction.Id));
                if (string.IsNullOrEmpty(Token))
                {
                    Token = GetTokenByUserID(NewTradeTransaction.MemberID.ToString());
                }
                if (!string.IsNullOrEmpty(Token))
                {
                    List<GetBuySellBook> list = new List<GetBuySellBook>();
                    if (OrderType != 4)
                    {
                        if (NewTradeTransaction.TrnType == 4)//Buy
                        {
                            list = _frontTrnRepository.GetBuyerBook(NewTradeTransaction.PairID, NewTradeTransaction.BidPrice);
                            foreach (var model in list)
                            {
                                BuySellmodel = model;
                                break;
                            }
                            if (BuySellmodel.OrderId.ToString() != "00000000-0000-0000-0000-000000000000")
                            {

                                Parallel.Invoke(() => BuyerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString()),
                                                () => HelperForLog.WriteLogForSocket("OnStatusCancel" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 BuyerBook Update call          TRNNO : " + Newtransaction.Id));
                            }
                            else
                            {
                                BuySellmodel.Amount = 0;
                                BuySellmodel.OrderId = new Guid();
                                BuySellmodel.RecordCount = 0;
                                BuySellmodel.Price = NewTradeTransaction.BidPrice;
                                Task.Run(() => Parallel.Invoke(() => BuyerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString()),
                                    () => HelperForLog.WriteLogForSocket("OnStatusCancel" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 BuyerBook pop call       TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName + " : No Data Found")));
                            }

                        }
                        else//Sell
                        {
                            list = _frontTrnRepository.GetSellerBook(NewTradeTransaction.PairID, NewTradeTransaction.AskPrice);
                            foreach (var model in list)
                            {

                                BuySellmodel = model;
                                break;
                            }
                            if (BuySellmodel.OrderId.ToString() != "00000000-0000-0000-0000-000000000000")
                            {
                                Parallel.Invoke(() => SellerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString()),
                                                () => HelperForLog.WriteLogForSocket("OnStatusCancel" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 SellerBook Update call         TRNNO : " + Newtransaction.Id));
                            }
                            else
                            {
                                BuySellmodel.Amount = 0;
                                BuySellmodel.OrderId = new Guid();
                                BuySellmodel.RecordCount = 0;
                                BuySellmodel.Price = NewTradeTransaction.AskPrice;
                                Task.Run(() => Parallel.Invoke(() => SellerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString()),
                                    () => HelperForLog.WriteLogForSocket("OnStatusCancel" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 SellerBook pop call          TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName + " : No Data Found")));
                            }
                        }
                    }
                    else
                    {
                        HelperForLog.WriteLogForSocket("OnStatusCancel" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 Order type 4 call OnLtpVhange    TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName);
                        this.OnLtpChange(0, NewTradeTransaction.PairID, NewTradeTransaction.PairName, IsCancel: 1, UserID: NewTradeTransaction.MemberID.ToString());
                    }

                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                    notification.MsgCode = Convert.ToInt32(enErrorCode.SignalRCancelOrder);
                    notification.Param1 = NewTradeTransaction.TrnNo.ToString();
                    notification.Type = Convert.ToInt16(EnNotificationType.Success);//rita 06-12-18 change from fail to success

                    if (IsPartialCancel == 0)//Fully Cancel
                    {
                        if (OrderType == 3) //for spot no open/recent order
                        {
                            if (OrderCount == 1)
                            {
                                Task.Run(() => Parallel.Invoke(() => ActivityNotificationV2(notification, Token, UserID)));
                            }
                            Task.Run(() => Parallel.Invoke(() => GetAndSendRecentOrderData(Newtransaction, NewTradeTransaction, Token, OrderType, UserID)));

                            //Task.Run(() => Parallel.Invoke(() => ActivityNotificationV2(notification, Token, UserID),
                            //    () => GetAndSendRecentOrderData(Newtransaction, NewTradeTransaction, Token, OrderType, UserID)));

                            Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusCancel" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "2 Complete Socket call    TRNNO : " + Newtransaction.Id));
                        }
                        else
                        {
                            if (OrderCount == 1)
                            {
                                Task.Run(() => Parallel.Invoke(() => ActivityNotificationV2(notification, Token, UserID)));
                            }
                            Task.Run(() => Parallel.Invoke(() => GetAndSendActiveOrderData(Newtransaction, NewTradeTransaction, Token, OrderType, UserID, 1),
                                    () => GetAndSendRecentOrderData(Newtransaction, NewTradeTransaction, Token, OrderType, UserID)));

                            //Task.Run(() => Parallel.Invoke(() => GetAndSendActiveOrderData(Newtransaction, NewTradeTransaction, Token, OrderType, UserID, 1),
                            //           () => GetAndSendRecentOrderData(Newtransaction, NewTradeTransaction, Token, OrderType, UserID),
                            //           () => ActivityNotificationV2(notification, Token, UserID)));
                            Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusCancel" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "2 Complete Socket call    TRNNO : " + Newtransaction.Id));
                        }

                    }
                    else if (IsPartialCancel == 1)//Partial Cancel
                    {
                        if(OrderCount == 1)
                        {
                            Task.Run(() => Parallel.Invoke(() => ActivityNotificationV2(notification, Token, UserID)));
                        }
                        Task.Run(() => Parallel.Invoke(() => GetAndSendActiveOrderData(Newtransaction, NewTradeTransaction, Token, OrderType, UserID, 1),
                                    () => GetAndSendRecentOrderData(Newtransaction, NewTradeTransaction, Token, OrderType, UserID)));


                        //Task.Run(() => Parallel.Invoke(() => GetAndSendActiveOrderData(Newtransaction, NewTradeTransaction, Token, OrderType, UserID, 1),
                        //                () => GetAndSendRecentOrderData(Newtransaction, NewTradeTransaction, Token, OrderType, UserID),
                        //                () => ActivityNotificationV2(notification, Token, UserID)));

                        Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusCancel  Fully+Cancel+Process" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "2 Complete Socket call    TRNNO : " + Newtransaction.Id));

                    }
                }
            }
            catch (Exception ex)
            {
                //_logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                HelperForLog.WriteErrorLog("OnStatusCancel :##TrnNo " + NewTradeTransaction.TrnNo, ControllerName, ex);
                
                //await this.OnStatusCancel(Status, Newtransaction, NewTradeTransaction, "", OrderType, IsPartialCancel);
            }
        }

        public async Task OnVolumeChange(VolumeDataRespose volumeData, MarketCapData capData, string UserID)
        {
            try
            {
                HelperForLog.WriteLogForSocket("OnVolumeChange" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "Call OnVolumeChangeMethod : volumeData : " + JsonConvert.SerializeObject(volumeData) + " : Market Data : " + JsonConvert.SerializeObject(capData));
                if (volumeData != null && capData != null)
                {
                    LastPriceViewModel lastPriceData = new LastPriceViewModel();
                    lastPriceData.LastPrice = capData.LastPrice;
                    lastPriceData.UpDownBit = volumeData.UpDownBit;

                    string Base = volumeData.PairName.Split("_")[1];
                    Task.Run(() => Parallel.Invoke(() => PairData(volumeData, Base, UserID),
                                    () => MarketData(capData, volumeData.PairName, UserID),
                                    () => LastPrice(lastPriceData, volumeData.PairName, UserID),
                                    () => HelperForLog.WriteLogForSocket("OnVolumeChange" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "After Last price Call Pair :" + volumeData.PairName + "  DATA :" + JsonConvert.SerializeObject(lastPriceData))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                
            }
        }

        public async Task OnWalletBalChange(WalletMasterResponse Data, string WalletTypeName, string Token, short TokenType = 1, string TrnNo = "", short IsMargin = 0) //ntrivedi 21-02-2018 added for margin wallet balance change
        {
            try
            {
                var MemberID = Token;
                if (TokenType == Convert.ToInt16(enTokenType.ByUserID))
                {
                    Token = GetTokenByUserID(Token);
                }
                if (!string.IsNullOrEmpty(Token))
                {
                    Task.Run(() => Parallel.Invoke(() => WalletBalUpdate(Data, WalletTypeName, Token, MemberID, IsMargin),
                                    () => HelperForLog.WriteLogForSocket("OnWalletBalChange" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, " Wallet Name : " + WalletTypeName + "         TRNNO : " + TrnNo.ToString() + " Member ID :" + MemberID + "   Data : " + JsonConvert.SerializeObject(Data) + " \n Token :" + Token)));
                }

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }

        public async Task RemoveActiveOrder(List<long> TrnList, long UserID)
        {
            try
            {
                RemoveActiveOrder RemoveObj = new RemoveActiveOrder();
                RemoveObj.Data = TrnList;
                Token = GetTokenByUserID(UserID.ToString());

                SignalRComm<RemoveActiveOrder> CommonData = new SignalRComm<RemoveActiveOrder>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.ActiveOrderRemove);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveActiveOrderRemove);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                CommonData.Data = RemoveObj;
                CommonData.Parameter = null;
                CommonData.IsMargin = 0;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.ActiveOrderRemove;
                SendData.Parameter = Token;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                var UserHub = _chat.RemoveActiveOrder(SendData.Parameter, SendData.DataObj);
                Task.Run(() => HelperForLog.WriteLogForSocket("RemoveActiveOrder", " ", " Data " + SendData.DataObj));
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }

        public async Task SendActivityNotificationV2(ActivityNotificationMessage ActivityNotification, string Token, short TokenType = 1, string TrnNo = "", short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {
                var MemberID = Token;

                //HelperForLog.WriteLogForSocket("SendActivityNotificationV2 " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, " 1 TRNNO : " + TrnNo.ToString() + "   Data : " + JsonConvert.SerializeObject(ActivityNotification) + " \n Token :" + Token);
                if (TokenType == Convert.ToInt16(enTokenType.ByUserID))
                {
                    Token = GetTokenByUserID(Token);
                }
                if (!string.IsNullOrEmpty(Token))
                {
                    ActivityNotificationV2(ActivityNotification, Token, MemberID, IsMargin);
                    Task.Run(() => HelperForLog.WriteLogForSocket("SendActivityNotificationV2 " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, " 2 TRNNO : " + TrnNo.ToString() + "   Data : " + JsonConvert.SerializeObject(ActivityNotification) + " \n Token :" + Token));
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }

        // khushali 12-01-2019
        public async Task SendWalletActivityList(ListAddWalletRequest ActivityListRequest, string ID, short IsMargin = 0)//Rita 20-2-19 for Margin Trading Data bit
        {
            try
            {

                //HelperForLog.WriteLogForSocket("ListAddWalletRequest " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, " 1 TRNNO : " + TrnNo.ToString() + "   Data : " + JsonConvert.SerializeObject(ActivityList) + " \n Token :" + Token);

                string Token = GetTokenByUserID(ID);
                if (!string.IsNullOrEmpty(Token))
                {
                    Task.Run(() => Parallel.Invoke(() => ActivityList(ActivityListRequest, Token, ID, IsMargin),
                    () => HelperForLog.WriteLogForSocket("SendActivityNotificationV2 " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "   Data : " + JsonConvert.SerializeObject(ActivityListRequest) + " \n Token :" + Token)));
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //throw ex;
            }
        }

        public async Task OnLtpChange(Decimal LTP, long Pair, string PairName, short IsCancel = 0, short IsMargin = 0, string UserID = "")//Rita 20-2-19 for Margin Trading Data bit
        {
            List<StopLimitBuySellBook> DataBuy = new List<StopLimitBuySellBook>();
            List<StopLimitBuySellBook> DataSell = new List<StopLimitBuySellBook>();
            try
            {
                HelperForLog.WriteLogForSocket("OnLtpChange" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, " LTP :" + LTP + " Pair :" + Pair);
                if (IsCancel == 0)
                {
                    if (IsMargin == 1)//Margin Trading
                    {
                        DataBuy = _frontTrnRepository.GetStopLimitBuySellBooksMargin(LTP, Pair, enOrderType.BuyOrder);
                        DataSell = _frontTrnRepository.GetStopLimitBuySellBooksMargin(LTP, Pair, enOrderType.SellOrder);
                    }
                    else
                    {
                        DataBuy = _frontTrnRepository.GetStopLimitBuySellBooks(LTP, Pair, enOrderType.BuyOrder);
                        DataSell = _frontTrnRepository.GetStopLimitBuySellBooks(LTP, Pair, enOrderType.SellOrder);
                    }
                }
                else if (IsCancel == 1)
                {
                    if (IsMargin == 1)//Margin Trading
                    {
                        DataBuy = _frontTrnRepository.GetStopLimitBuySellBooksMargin(LTP, Pair, enOrderType.BuyOrder, 1);
                        DataSell = _frontTrnRepository.GetStopLimitBuySellBooksMargin(LTP, Pair, enOrderType.SellOrder, 1);
                    }
                    else
                    {
                        DataBuy = _frontTrnRepository.GetStopLimitBuySellBooks(LTP, Pair, enOrderType.BuyOrder, 1);
                        DataSell = _frontTrnRepository.GetStopLimitBuySellBooks(LTP, Pair, enOrderType.SellOrder, 1);
                    }
                }
                Task.Run(() => StopLimitBuyerBook(DataBuy, PairName, UserID, IsMargin));
                Task.Run(() => StopLimitSellerBook(DataSell, PairName, UserID, IsMargin));
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }
        #endregion

        //Rita 5-3-19 for margin trading
        #region margin Global Event trading

        public async Task OnStatusSuccessMargin(short Status, TransactionQueueMargin Newtransaction, TradeTransactionQueueMargin NewTradeTransaction, string Token, short OrderType, decimal SettlementPrice)
        {
            //update Recent Order
            //pop OpenOrder
            //add tradehistory
            //add orderhistory
            //pop buyer/seller book;
            string UserID = NewTradeTransaction.MemberID.ToString();
            try
            {
                GetBuySellBook BuySellmodel = new GetBuySellBook();

                Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusSuccessMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "0 start Socket call       TRNNO : " + Newtransaction.Id));
                if (string.IsNullOrEmpty(Token))
                {
                    Token = GetTokenByUserID(NewTradeTransaction.MemberID.ToString());
                }
                if (!string.IsNullOrEmpty(Token))
                {
                    List<GetBuySellBook> list = new List<GetBuySellBook>();
                    if (NewTradeTransaction.TrnType == 4)//Buy
                    {
                        list = _frontTrnRepository.GetBuyerBookMargin(NewTradeTransaction.PairID, NewTradeTransaction.BidPrice);
                        foreach (var model in list)
                        {
                            BuySellmodel = model;
                            break;
                        }
                        if (BuySellmodel.OrderId.ToString() != "00000000-0000-0000-0000-000000000000")
                        {

                            Parallel.Invoke(() => BuyerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString(), IsMargin: 1),
                                            () => HelperForLog.WriteLogForSocket("OnStatusSuccessMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 BuyerBook Update call          TRNNO : " + Newtransaction.Id));
                        }
                        else
                        {
                            BuySellmodel.Amount = 0;
                            BuySellmodel.OrderId = new Guid();
                            BuySellmodel.RecordCount = 0;
                            BuySellmodel.Price = NewTradeTransaction.BidPrice;
                            Task.Run(() => Parallel.Invoke(() => BuyerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString(), IsMargin: 1),
                                () => HelperForLog.WriteLogForSocket("OnStatusSuccessMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 BuyerBook pop call       TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName + " : No Data Found")));
                        }

                    }
                    else//Sell
                    {
                        list = _frontTrnRepository.GetSellerBookMargin(NewTradeTransaction.PairID, NewTradeTransaction.AskPrice);
                        foreach (var model in list)
                        {

                            BuySellmodel = model;
                            break;
                        }
                        if (BuySellmodel.OrderId.ToString() != "00000000-0000-0000-0000-000000000000")
                        {
                            Parallel.Invoke(() => SellerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString(), IsMargin: 1),
                                            () => HelperForLog.WriteLogForSocket("OnStatusSuccessMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 SellerBook Update call         TRNNO : " + Newtransaction.Id));
                        }
                        else
                        {
                            BuySellmodel.Amount = 0;
                            BuySellmodel.OrderId = new Guid();
                            BuySellmodel.RecordCount = 0;
                            BuySellmodel.Price = NewTradeTransaction.AskPrice;
                            Task.Run(() => Parallel.Invoke(() => SellerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString(), IsMargin: 1),
                                () => HelperForLog.WriteLogForSocket("OnStatusSuccessMargin" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 SellerBook pop call          TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName + " : No Data Found")));
                        }

                    }
                   
                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                    var Price= (NewTradeTransaction.TrnType == 4) ? (SettlementPrice == 0 ? NewTradeTransaction.BidPrice : SettlementPrice) : NewTradeTransaction.AskPrice;
                    var Amount= (NewTradeTransaction.TrnType == 4) ? NewTradeTransaction.BuyQty: NewTradeTransaction.SellQty;
                    var Charge = Convert.ToDecimal(Newtransaction.ChargeRs == null ? 0 : Newtransaction.ChargeRs);
                    notification.MsgCode = Convert.ToInt32(enErrorCode.SignalRTrnSuccessfullySettled);
                    notification.Param1 = Price.ToString();
                    notification.Param2 = Amount.ToString();
                    notification.Param3 =Convert.ToString(NewTradeTransaction.TrnTypeName == "BUY" ? ((Price * Amount) - Charge) : ((Price * Amount)));
                    notification.Type = Convert.ToInt16(EnNotificationType.Success);
                    if (OrderType == 3)
                    {
                        Task.Run(() => Parallel.Invoke(() => GetAndSendRecentOrderDataMargin(Newtransaction, NewTradeTransaction, Token, OrderType, UserID).Wait(),//rita 26-7-19 added wait
                       // () => GetAndSendTradeHistoryInfoDataMargin(Newtransaction, NewTradeTransaction, Token,UserID,SettlementPrice:SettlementPrice).Wait(),//rita 26-7-19 added wait
                        () => ActivityNotificationV2(notification, Token, UserID, 1))).Wait();//rita 26-7-19 added wait
                        GetAndSendTradeHistoryInfoDataMargin(Newtransaction, NewTradeTransaction, Token, UserID, SettlementPrice: SettlementPrice).Wait();
                    }
                    else
                    {
                        Task.Run(() => Parallel.Invoke(() => GetAndSendRecentOrderDataMargin(Newtransaction, NewTradeTransaction, Token, OrderType, UserID).Wait(),//rita 26-7-19 added wait
                        () => GetAndSendActiveOrderDataMargin(Newtransaction, NewTradeTransaction, Token, OrderType, UserID, 1),
                                  // () => GetAndSendTradeHistoryInfoDataMargin(Newtransaction, NewTradeTransaction, Token, UserID, SettlementPrice: SettlementPrice).Wait(),//rita 26-7-19 added wait
                        () => ActivityNotificationV2(notification, Token, UserID, 1))).Wait();//rita 26-7-19 added wait
                        GetAndSendTradeHistoryInfoDataMargin(Newtransaction, NewTradeTransaction, Token, UserID, SettlementPrice: SettlementPrice).Wait();
                    }
                    Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusSuccessMargin" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "2 Complete Socket call    TRNNO : " + Newtransaction.Id));

                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("OnStatusSuccessMargin ##TrnNo:" + NewTradeTransaction.TrnNo, ControllerName, ex);
            }
        }

        public async Task OnStatusPartialSuccessMargin(short Status, TransactionQueueMargin Newtransaction, TradeTransactionQueueMargin NewTradeTransaction, string Token, short OrderType)
        {
            //update Buyer/seller book
            string UserID = NewTradeTransaction.MemberID.ToString();
            try
            {
                GetBuySellBook BuySellmodel = new GetBuySellBook();
                //HelperForLog.WriteLogForSocket("OnStatusPartialSuccess", ControllerName, " TransactionQueue :" + JsonConvert.SerializeObject(Newtransaction) + " TradeTransactionQueue :" + JsonConvert.SerializeObject(NewTradeTransaction));
                if (string.IsNullOrEmpty(Token))
                {
                    Token = GetTokenByUserID(NewTradeTransaction.MemberID.ToString());
                }
                if (!string.IsNullOrEmpty(Token))
                {
                    List<GetBuySellBook> list = new List<GetBuySellBook>();
                    if (NewTradeTransaction.TrnType == 4)//Buy
                    {
                        list = _frontTrnRepository.GetBuyerBookMargin(NewTradeTransaction.PairID, NewTradeTransaction.BidPrice);
                        foreach (var model in list)
                        {
                            BuySellmodel = model;
                            break;
                        }
                        if (BuySellmodel.OrderId.ToString() != "00000000-0000-0000-0000-000000000000")
                        {

                            Parallel.Invoke(() => BuyerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString(), IsMargin: 1),
                                            () => HelperForLog.WriteLogForSocket("OnStatusPartialSuccessMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 BuyerBook call          TRNNO : " + Newtransaction.Id));
                        }
                        else
                            Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusPartialSuccessMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 BuyerBook call          TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName + " : No Data Found"));
                    }
                    else//Sell
                    {
                        list = _frontTrnRepository.GetSellerBookMargin(NewTradeTransaction.PairID, NewTradeTransaction.AskPrice);
                        foreach (var model in list)
                        {

                            BuySellmodel = model;
                            break;
                        }
                        if (BuySellmodel.OrderId.ToString() != "00000000-0000-0000-0000-000000000000")
                        {
                            Parallel.Invoke(() => SellerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString(), IsMargin: 1),
                                            () => HelperForLog.WriteLogForSocket("OnStatusPartialSuccessMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 SellerBook call         TRNNO : " + Newtransaction.Id));
                        }
                        else
                            Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusPartialSuccessMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 SellerBook call         TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName + " : No Data Found"));
                    }
                    //Rita 13-3-19 for Settled Qty update
                    if (OrderType != 3)//for market ordre not sent open and recent ordre 
                    {

                        Task.Run(() => Parallel.Invoke(() => GetAndSendActiveOrderDataMargin(Newtransaction, NewTradeTransaction, Token, OrderType, UserID).Wait(),//rita 26-7-19 added wait
                        () => GetAndSendRecentOrderDataMargin(Newtransaction, NewTradeTransaction, Token, OrderType, UserID))).Wait();//rita 26-7-19 added wait
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("OnStatusPartialSuccessMargin ##TrnNo:" + NewTradeTransaction.TrnNo, ControllerName, ex);
            }
        }

        public async Task OnStatusHoldMargin(short Status, TransactionQueueMargin Newtransaction, TradeTransactionQueueMargin NewTradeTransaction, string Token, short OrderType)
        {
            //add buyer/seller book
            //add OpenOrder
            //add recent order
            string UserID = NewTradeTransaction.MemberID.ToString();
            try
            {
                GetBuySellBook BuySellmodel = new GetBuySellBook();
                Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusHoldMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "0 start Socket call       TRNNO : " + Newtransaction.Id));
                if (string.IsNullOrEmpty(Token))
                {
                    Token = GetTokenByUserID(NewTradeTransaction.MemberID.ToString());
                }
                List<GetBuySellBook> list = new List<GetBuySellBook>();
                if (!string.IsNullOrEmpty(Token))
                {
                    if (OrderType == 4)
                    {
                        HelperForLog.WriteLogForSocket("OnStatusHoldMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, " Order type 4 call OnLtpVhange    TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName);
                        this.OnLtpChange(0, NewTradeTransaction.PairID, NewTradeTransaction.PairName, IsCancel: 1, IsMargin: 1, UserID: NewTradeTransaction.MemberID.ToString());
                    }
                    else
                    {
                        if (NewTradeTransaction.TrnType == 4)//Buy
                        {
                            list = _frontTrnRepository.GetBuyerBookMargin(NewTradeTransaction.PairID, NewTradeTransaction.BidPrice);
                            foreach (var model in list)
                            {
                                //BuySellmodel = model;
                                BuySellmodel.Amount = model.Amount;
                                BuySellmodel.Price = model.Price;
                                BuySellmodel.OrderId = model.OrderId;
                                BuySellmodel.RecordCount = model.RecordCount;
                                break;
                            }
                            if (BuySellmodel.OrderId.ToString() != "00000000-0000-0000-0000-000000000000")
                            {
                                Parallel.Invoke(() => BuyerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString(), IsMargin: 1),
                                                () => HelperForLog.WriteLogForSocket("OnStatusHoldMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 BuyerBook call          TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName + " BuyerBook Amount " + BuySellmodel.Amount + " Price " + BuySellmodel.Price));
                            }
                            else
                                Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusHoldMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 BuyerBook call          TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName + " : No Data Found"));

                        }
                        else//Sell
                        {
                            list = _frontTrnRepository.GetSellerBookMargin(NewTradeTransaction.PairID, NewTradeTransaction.AskPrice);
                            foreach (var model in list)
                            {
                                //BuySellmodel = model;
                                BuySellmodel.Price = model.Price;
                                BuySellmodel.Amount = model.Amount;
                                BuySellmodel.OrderId = model.OrderId;
                                BuySellmodel.RecordCount = model.RecordCount;
                                break;
                            }
                            if (BuySellmodel.OrderId.ToString() != "00000000-0000-0000-0000-000000000000")
                            {
                                Parallel.Invoke(() => SellerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString(), IsMargin: 1),
                                                () => HelperForLog.WriteLogForSocket("OnStatusHoldMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 SellerBook call         TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName + " SellerBook Amount " + BuySellmodel.Amount + " Price " + BuySellmodel.Price));
                            }
                            else
                                Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusHoldMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 SellerBook call         TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName + " : No Data Found"));

                        }
                    }
                    if (OrderType != 3)//for market ordre not sent open and recent ordre 
                    {
                        Task.Run(() => Parallel.Invoke(() => GetAndSendActiveOrderDataMargin(Newtransaction, NewTradeTransaction, Token, OrderType, UserID).Wait(),//rita 26-7-19 added wait
                        () => GetAndSendRecentOrderDataMargin(Newtransaction, NewTradeTransaction, Token, OrderType, UserID))).Wait();//rita 26-7-19 added wait
                    }
                    Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusHoldMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "2 Complete Socket call    TRNNO : " + Newtransaction.Id));
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("OnStatusHoldMargin ##TrnNo:" + NewTradeTransaction.TrnNo, ControllerName, ex);
            }
        }

        public async Task OnStatusCancelMargin(short Status, TransactionQueueMargin Newtransaction, TradeTransactionQueueMargin NewTradeTransaction, string Token, short OrderType, short IsPartialCancel = 0)
        {
            //pop from OpenOrder
            //update Recent order
            //Buyer/Seller pop
            string UserID = NewTradeTransaction.MemberID.ToString();
            try
            {
                GetBuySellBook BuySellmodel = new GetBuySellBook();
                Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusCancelMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "0 start Socket call       TRNNO : " + Newtransaction.Id));
                if (string.IsNullOrEmpty(Token))
                {
                    Token = GetTokenByUserID(NewTradeTransaction.MemberID.ToString());
                }
                if (!string.IsNullOrEmpty(Token))
                {
                    List<GetBuySellBook> list = new List<GetBuySellBook>();
                    if (OrderType != 4)
                    {
                        if (NewTradeTransaction.TrnType == 4)//Buy
                        {
                            list = _frontTrnRepository.GetBuyerBookMargin(NewTradeTransaction.PairID, NewTradeTransaction.BidPrice);
                            foreach (var model in list)
                            {
                                BuySellmodel = model;
                                break;
                            }
                            if (BuySellmodel.OrderId.ToString() != "00000000-0000-0000-0000-000000000000")
                            {

                                Parallel.Invoke(() => BuyerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString(), IsMargin: 1),
                                                () => HelperForLog.WriteLogForSocket("OnStatusCancelMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 BuyerBook Update call          TRNNO : " + Newtransaction.Id));
                            }
                            else
                            {
                                BuySellmodel.Amount = 0;
                                BuySellmodel.OrderId = new Guid();
                                BuySellmodel.RecordCount = 0;
                                BuySellmodel.Price = NewTradeTransaction.BidPrice;
                                Task.Run(() => Parallel.Invoke(() => BuyerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString(), IsMargin: 1),
                                    () => HelperForLog.WriteLogForSocket("OnStatusCancelMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 BuyerBook pop call       TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName + " : No Data Found")));
                            }

                        }
                        else//Sell
                        {
                            list = _frontTrnRepository.GetSellerBookMargin(NewTradeTransaction.PairID, NewTradeTransaction.AskPrice);
                            foreach (var model in list)
                            {

                                BuySellmodel = model;
                                break;
                            }
                            if (BuySellmodel.OrderId.ToString() != "00000000-0000-0000-0000-000000000000")
                            {
                                Parallel.Invoke(() => SellerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString(), IsMargin: 1),
                                                () => HelperForLog.WriteLogForSocket("OnStatusCancelMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 SellerBook Update call         TRNNO : " + Newtransaction.Id));
                            }
                            else
                            {
                                BuySellmodel.Amount = 0;
                                BuySellmodel.OrderId = new Guid();
                                BuySellmodel.RecordCount = 0;
                                BuySellmodel.Price = NewTradeTransaction.AskPrice;
                                Task.Run(() => Parallel.Invoke(() => SellerBook(BuySellmodel, NewTradeTransaction.PairName, NewTradeTransaction.MemberID.ToString(), IsMargin: 1),
                                    () => HelperForLog.WriteLogForSocket("OnStatusCancelMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 SellerBook pop call          TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName + " : No Data Found")));
                            }
                        }
                    }
                    else
                    {
                        HelperForLog.WriteLogForSocket("OnStatusCancelMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 Order type 4 call OnLtpVhange    TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName);
                        this.OnLtpChange(0, NewTradeTransaction.PairID, NewTradeTransaction.PairName, UserID: NewTradeTransaction.MemberID.ToString(), IsCancel: 1, IsMargin: 1);
                    }
                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                    notification.MsgCode = Convert.ToInt32(enErrorCode.SignalRCancelOrder);
                    notification.Param1 = NewTradeTransaction.TrnNo.ToString();
                    notification.Type = Convert.ToInt16(EnNotificationType.Success);//rita 06-12-18 change from fail to success
                    if (IsPartialCancel == 0)//Fully Cancel
                    {
                        if (OrderType == 3) //for spot no open/recent order
                        {
                            Task.Run(() => Parallel.Invoke(() => ActivityNotificationV2(notification, Token, UserID, 1).Wait(),//rita 26-7-19 added wait
                            () => GetAndSendRecentOrderDataMargin(Newtransaction, NewTradeTransaction, Token, OrderType, UserID))).Wait();//rita 26-7-19 added wait
                            Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusCancelMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "2 Complete Socket call    TRNNO : " + Newtransaction.Id));
                        }
                        else
                        {
                            Task.Run(() => Parallel.Invoke(() => GetAndSendActiveOrderDataMargin(Newtransaction, NewTradeTransaction, Token, OrderType, UserID, 1).Wait(),//rita 26-7-19 added wait
                            () => GetAndSendRecentOrderDataMargin(Newtransaction, NewTradeTransaction, Token, OrderType, UserID).Wait(),//rita 26-7-19 added wait
                            () => ActivityNotificationV2(notification, Token, UserID, 1))).Wait();//rita 26-7-19 added wait
                            Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusCancelMargin " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "2 Complete Socket call    TRNNO : " + Newtransaction.Id));
                        }

                    }
                    else if (IsPartialCancel == 1)//Partial Cancel
                    {
                        Task.Run(() => Parallel.Invoke(() => GetAndSendActiveOrderDataMargin(Newtransaction, NewTradeTransaction, Token, OrderType, UserID, 1).Wait(),//rita 26-7-19 added wait
                        () => GetAndSendRecentOrderDataMargin(Newtransaction, NewTradeTransaction, Token, OrderType, UserID).Wait(),//rita 26-7-19 added wait
                        () => ActivityNotificationV2(notification, Token, UserID, 1))).Wait();//rita 26-7-19 added wait

                        Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusCancelMargin  Fully+Cancel+Process" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "2 Complete Socket call    TRNNO : " + Newtransaction.Id));
                    }
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("OnStatusCancelMargin ##TrnNo:" + NewTradeTransaction.TrnNo, ControllerName, ex);
            }
        }

        public async Task OnVolumeChangeMargin(VolumeDataRespose volumeData, MarketCapData capData, string UserID)
        {
            try
            {
                HelperForLog.WriteLogForSocket("OnVolumeChangeMargin" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "Call OnVolumeChangeMethod : volumeData : " + JsonConvert.SerializeObject(volumeData) + " : Market Data : " + JsonConvert.SerializeObject(capData));
                if (volumeData != null && capData != null)
                {
                    LastPriceViewModel lastPriceData = new LastPriceViewModel();
                    lastPriceData.LastPrice = capData.LastPrice;
                    lastPriceData.UpDownBit = volumeData.UpDownBit;

                    string Base = volumeData.PairName.Split("_")[1];

                    Task.Run(() => Parallel.Invoke(() => PairData(volumeData, Base, UserID, IsMargin: 1),
                                    () => MarketData(capData, volumeData.PairName, UserID, IsMargin: 1),
                                    () => LastPrice(lastPriceData, volumeData.PairName, UserID, IsMargin: 1),
                                    () => HelperForLog.WriteLogForSocket("OnVolumeChangeMargin" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "After Last price Call Pair :" + volumeData.PairName + "  DATA :" + JsonConvert.SerializeObject(lastPriceData))));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:OnVolumeChangeMargin" + "\nClassname=" + this.GetType().Name, LogLevel.Error);
            }
        }

        #endregion

        #region HelperMethods
        public async Task GetAndSendActiveOrderData(TransactionQueue Newtransaction, TradeTransactionQueue NewTradeTransaction, string Token, short OrderType, string UserID, short IsPop = 0)
        {
            try
            {
                ActiveOrderInfoV1 activeOrder = new ActiveOrderInfoV1();
                activeOrder.GUID = Newtransaction.GUID.ToString();
                activeOrder.Id = Newtransaction.Id.ToString();
                activeOrder.TrnDate = Newtransaction.TrnDate;
                activeOrder.Type = (NewTradeTransaction.TrnType == 4) ? "BUY" : "SELL";
                activeOrder.Order_Currency = NewTradeTransaction.Order_Currency;
                activeOrder.Delivery_Currency = NewTradeTransaction.Delivery_Currency;
                if (IsPop == 1)
                    activeOrder.Amount = 0;
                else
                    activeOrder.Amount = (NewTradeTransaction.BuyQty == 0) ? NewTradeTransaction.SellQty : (NewTradeTransaction.SellQty == 0) ? NewTradeTransaction.BuyQty : NewTradeTransaction.BuyQty;
                activeOrder.Price = (NewTradeTransaction.BidPrice == 0) ? NewTradeTransaction.AskPrice : (NewTradeTransaction.AskPrice == 0) ? NewTradeTransaction.BidPrice : NewTradeTransaction.BidPrice;
                activeOrder.IsCancelled = NewTradeTransaction.IsCancelled;
                activeOrder.OrderType = Enum.GetName(typeof(enTransactionMarketType), OrderType);
                activeOrder.PairId = NewTradeTransaction.PairID;
                activeOrder.PairName = NewTradeTransaction.PairName;
                //Rita 12-3-19 this required for front side
                activeOrder.SettledQty = (NewTradeTransaction.TrnType == 4) ? NewTradeTransaction.SettledBuyQty : NewTradeTransaction.SettledSellQty;
                activeOrder.SettledDate = NewTradeTransaction.SettledDate;
                activeOrder.ChargeRs = Convert.ToDecimal(Newtransaction.ChargeRs == null ? 0 : Newtransaction.ChargeRs);
                activeOrder.Chargecurrency = string.IsNullOrEmpty(Newtransaction.ChargeCurrency) ? "" : Newtransaction.ChargeCurrency;
                if (IsPop != 1)//send notification,not pop call
                {
                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                    notification.MsgCode = Convert.ToInt32(enErrorCode.SignalRTrnSuccessfullyCreated);
                    notification.Param1 = activeOrder.Price.ToString();
                    notification.Param2 = activeOrder.Amount.ToString();
                    notification.Type = Convert.ToInt16(EnNotificationType.Success);
                    //komal 11-11-2019 12:12 PM remove unwanted alert
                    Task.Run(() =>
                        Parallel.Invoke(() => ActiveOrder(activeOrder, Token, UserID)
                                    //() => ActivityNotificationV2(notification, Token, UserID)
                                    ));
                }
                else
                    ActiveOrder(activeOrder, Token, UserID);
                HelperForLog.WriteLogForSocket("GetAndSendActiveOrderData", ControllerName, " 1 ActiveOrder call TRNNO:" + Newtransaction.Id + " Order Type " + OrderType);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //throw ex;
            }
        }

        public async Task GetAndSendTradeHistoryInfoData(TransactionQueue Newtransaction, TradeTransactionQueue NewTradeTransaction, string Token, string UserID,  decimal SettlementPrice = 0)
        {
            try
            {
                GetTradeHistoryInfoV1 model = new GetTradeHistoryInfoV1();
                model.TrnNo = Newtransaction.Id.ToString();
                model.GUID = Newtransaction.GUID.ToString();
                model.Type = (NewTradeTransaction.TrnType == 4) ? "BUY" : "SELL";

                //Rita 09-4-09 in case of buy send settlement price for OrderHistory in front side , LTP and History Diff.price issue solved
                model.Price = (NewTradeTransaction.TrnType == 4) ? (SettlementPrice == 0 ? NewTradeTransaction.BidPrice : SettlementPrice) : NewTradeTransaction.AskPrice;
                model.Amount = (NewTradeTransaction.TrnType == 4) ? NewTradeTransaction.BuyQty : NewTradeTransaction.SellQty; //Rita 19-11-18 May be Qty not fully sell from Pool

                //komal 30 April 2019 add charge
                //model.ChargeRs = Newtransaction.ChargeRs;
                model.ChargeRs = Convert.ToDecimal(Newtransaction.ChargeRs == null ? 0 : Newtransaction.ChargeRs);
                model.Chargecurrency = string.IsNullOrEmpty(Newtransaction.ChargeCurrency) ? "" : Newtransaction.ChargeCurrency;
                model.Total = model.Type == "BUY" ? ((model.Price * model.Amount) -model.ChargeRs) : ((model.Price * model.Amount));
                model.DateTime = Convert.ToDateTime(NewTradeTransaction.SettledDate);
                model.Status = NewTradeTransaction.Status;
                model.StatusText = Enum.GetName(typeof(enTransactionStatus), model.Status);
                model.PairName = NewTradeTransaction.PairName;
                
                model.IsCancel = NewTradeTransaction.IsCancelled;
                model.OrderType = Enum.GetName(typeof(enTransactionMarketType), NewTradeTransaction.ordertype);
                model.SettledDate = NewTradeTransaction.SettledDate;
                model.SettledQty = (NewTradeTransaction.TrnType == 4) ? NewTradeTransaction.SettledBuyQty : NewTradeTransaction.SettledSellQty;
               
                model.SettlementPrice = _frontTrnRepository.GetTradeSettlementPrice(NewTradeTransaction.TrnNo).SettlementPrice;
                TradeHistory(model, Token, UserID, 0);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }
        
        public async Task GetAndSendRecentOrderData(TransactionQueue Newtransaction, TradeTransactionQueue NewTradeTransaction, string Token, short OrderType, string UserID, short IsPop = 0)
        {
            try
            {
                RecentOrderInfoV1 model = new RecentOrderInfoV1();
                model.TrnNo = Newtransaction.Id.ToString();
                model.GUID = Newtransaction.GUID.ToString();
                model.Type = (NewTradeTransaction.TrnType == 4) ? "BUY" : "SELL";
                model.Price = (NewTradeTransaction.BidPrice == 0) ? NewTradeTransaction.AskPrice : (NewTradeTransaction.AskPrice == 0) ? NewTradeTransaction.BidPrice : NewTradeTransaction.BidPrice;
                model.Qty = (NewTradeTransaction.TrnType == 4) ? NewTradeTransaction.BuyQty : NewTradeTransaction.SellQty; ;
                model.DateTime = NewTradeTransaction.TrnDate;
                model.Status = Enum.GetName(typeof(enTransactionStatus), NewTradeTransaction.Status);
                model.PairId = NewTradeTransaction.PairID;
                model.PairName = NewTradeTransaction.PairName;
                model.OrderType = Enum.GetName(typeof(enTransactionMarketType), OrderType);
                model.StatusCode = NewTradeTransaction.Status;
                model.IsCancel = NewTradeTransaction.IsCancelled;//Rita 22-3-19 added for separate status with success in case of partial cancel
                model.SettledDate = NewTradeTransaction.SettledDate;
                model.SettledQty = (NewTradeTransaction.TrnType == 4) ? NewTradeTransaction.SettledBuyQty : NewTradeTransaction.SettledSellQty;
                model.SettlementPrice = _frontTrnRepository.GetTradeSettlementPrice(NewTradeTransaction.TrnNo).SettlementPrice;
                model.ChargeRs = Convert.ToDecimal(Newtransaction.ChargeRs == null ? 0 : Newtransaction.ChargeRs);
                model.Chargecurrency = string.IsNullOrEmpty(Newtransaction.ChargeCurrency) ? "" : Newtransaction.ChargeCurrency;
                RecentOrder(model, Token, UserID);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                
            }
        }

        //Rita 20-2-19 for margin trading

        public async Task GetAndSendActiveOrderDataMargin(TransactionQueueMargin Newtransaction, TradeTransactionQueueMargin NewTradeTransaction, string Token, short OrderType, string UserID, short IsPop = 0)
        {
            try
            {
                //Rita 01-05-19 for margin trading
                if (NewTradeTransaction.ordertype == 4 && NewTradeTransaction.IsWithoutAmtHold == 1 && NewTradeTransaction.ISOrderBySystem == 1)
                {
                    HelperForLog.WriteLogForSocket("GetAndSendActiveOrderDataMargin", ControllerName, " skip this system Order ##TrnNo:" + Newtransaction.Id + " Order Type " + OrderType);
                    return;
                }
                ActiveOrderInfoV1 activeOrder = new ActiveOrderInfoV1();
                activeOrder.Id =Newtransaction.Id.ToString();
                activeOrder.GUID = Newtransaction.GUID.ToString();
                activeOrder.TrnDate = Newtransaction.TrnDate;
                activeOrder.Type = (NewTradeTransaction.TrnType == 4) ? "BUY" : "SELL";
                activeOrder.Order_Currency = NewTradeTransaction.Order_Currency;
                activeOrder.Delivery_Currency = NewTradeTransaction.Delivery_Currency;
                if (IsPop == 1)
                    activeOrder.Amount = 0;
                else
                    activeOrder.Amount = (NewTradeTransaction.BuyQty == 0) ? NewTradeTransaction.SellQty : (NewTradeTransaction.SellQty == 0) ? NewTradeTransaction.BuyQty : NewTradeTransaction.BuyQty;
                activeOrder.Price = (NewTradeTransaction.BidPrice == 0) ? NewTradeTransaction.AskPrice : (NewTradeTransaction.AskPrice == 0) ? NewTradeTransaction.BidPrice : NewTradeTransaction.BidPrice;
                activeOrder.IsCancelled = NewTradeTransaction.IsCancelled;
                activeOrder.OrderType = Enum.GetName(typeof(enTransactionMarketType), OrderType);
                activeOrder.PairId = NewTradeTransaction.PairID;
                activeOrder.PairName = NewTradeTransaction.PairName;
                //Rita 12-3-19 this required for front side
                activeOrder.SettledQty = (NewTradeTransaction.TrnType == 4) ? NewTradeTransaction.SettledBuyQty : NewTradeTransaction.SettledSellQty;
                activeOrder.SettledDate = NewTradeTransaction.SettledDate;

                if (IsPop != 1)//send notification,not pop call
                {
                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                    notification.MsgCode = Convert.ToInt32(enErrorCode.SignalRTrnSuccessfullyCreated);
                    notification.Param1 = activeOrder.Price.ToString();
                    notification.Param2 = activeOrder.Amount.ToString();
                    notification.Type = Convert.ToInt16(EnNotificationType.Success);
                    //komal 11-11-2019 12:12 PM remove unwanted alert
                    Task.Run(() =>
                        Parallel.Invoke(() => ActiveOrder(activeOrder, Token, UserID, 1)
                                    //() => ActivityNotificationV2(notification, Token, UserID, 1)
                                    ));
                }
                else
                {
                    ActiveOrder(activeOrder, Token, UserID, 1);
                }
                HelperForLog.WriteLogForSocket("GetAndSendActiveOrderDataMargin", ControllerName, " 1 ActiveOrder call TRNNO:" + Newtransaction.Id + " Order Type " + OrderType);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetAndSendActiveOrderDataMargin ##TrnNo:" + NewTradeTransaction.TrnNo, ControllerName, ex);
            }
        }

        public async Task GetAndSendTradeHistoryInfoDataMargin(TransactionQueueMargin Newtransaction, TradeTransactionQueueMargin NewTradeTransaction, string Token, string UserID, decimal SettlementPrice = 0)
        {
            try
            {
                GetTradeHistoryInfoV1 model = new GetTradeHistoryInfoV1();
                model.TrnNo = Newtransaction.Id.ToString();
                model.GUID = Newtransaction.GUID.ToString();
                model.Type = (NewTradeTransaction.TrnType == 4) ? "BUY" : "SELL";
                //Rita 09-4-09 in case of buy send settlement price for OrderHistory in front side , LTP and History Diff.price issue solved
                model.Price = (NewTradeTransaction.TrnType == 4) ? (SettlementPrice == 0 ? NewTradeTransaction.BidPrice : SettlementPrice) : NewTradeTransaction.AskPrice;
                model.Amount = (NewTradeTransaction.TrnType == 4) ? NewTradeTransaction.BuyQty : NewTradeTransaction.SellQty; //Rita 19-11-18 May be Qty not fully sell from Pool
                //komal 30 April 2019 add charge
                model.ChargeRs = Convert.ToDecimal(Newtransaction.ChargeRs==null? 0 : Newtransaction.ChargeRs);
                model.Chargecurrency = string.IsNullOrEmpty(Newtransaction.ChargeCurrency) ? "" : Newtransaction.ChargeCurrency;
                model.Total = model.Type == "BUY" ? (model.Price * model.Amount) - model.ChargeRs : (model.Price * model.Amount);
                model.DateTime = Convert.ToDateTime(NewTradeTransaction.SettledDate);
                model.Status = NewTradeTransaction.Status;
                model.StatusText = Enum.GetName(typeof(enTransactionStatus), model.Status);
                model.PairName = NewTradeTransaction.PairName;
                model.IsCancel = NewTradeTransaction.IsCancelled;
                model.OrderType = Enum.GetName(typeof(enTransactionMarketType), NewTradeTransaction.ordertype);
                model.SettledDate = NewTradeTransaction.SettledDate;
                model.SettledQty = (NewTradeTransaction.TrnType == 4) ? NewTradeTransaction.SettledBuyQty : NewTradeTransaction.SettledSellQty;
                model.SettlementPrice = _frontTrnRepository.GetTradeSettlementPriceMargin(NewTradeTransaction.TrnNo).SettlementPrice;
                TradeHistory(model, Token, UserID, 1);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetAndSendTradeHistoryInfoDataMargin ##TrnNo:" + NewTradeTransaction.TrnNo, ControllerName, ex);
            }
        }

        public async Task GetAndSendRecentOrderDataMargin(TransactionQueueMargin Newtransaction, TradeTransactionQueueMargin NewTradeTransaction, string Token, short OrderType, string UserID, short IsPop = 0)
        {
            try
            {
                //Rita 01-05-19 for margin trading
                if (NewTradeTransaction.ordertype == 4 && NewTradeTransaction.IsWithoutAmtHold == 1 && NewTradeTransaction.ISOrderBySystem == 1)
                {
                    HelperForLog.WriteLogForSocket("GetAndSendRecentOrderDataMargin", ControllerName, " skip this system Order ##TrnNo:" + Newtransaction.Id + " Order Type " + OrderType);
                    return;
                }
                RecentOrderInfoV1 model = new RecentOrderInfoV1();
                model.TrnNo = Newtransaction.Id.ToString();
                model.GUID = Newtransaction.GUID.ToString();
                model.Type = (NewTradeTransaction.TrnType == 4) ? "BUY" : "SELL";
                model.Price = (NewTradeTransaction.BidPrice == 0) ? NewTradeTransaction.AskPrice : (NewTradeTransaction.AskPrice == 0) ? NewTradeTransaction.BidPrice : NewTradeTransaction.BidPrice;
                model.Qty = (NewTradeTransaction.TrnType == 4) ? NewTradeTransaction.BuyQty : NewTradeTransaction.SellQty; ;
                model.DateTime = NewTradeTransaction.TrnDate;
                model.Status = Enum.GetName(typeof(enTransactionStatus), NewTradeTransaction.Status);
                model.PairId = NewTradeTransaction.PairID;
                model.PairName = NewTradeTransaction.PairName;
                model.OrderType = Enum.GetName(typeof(enTransactionMarketType), OrderType);
                model.StatusCode = NewTradeTransaction.Status;
                model.IsCancel = NewTradeTransaction.IsCancelled;//Rita 22-3-19 added for separate status with success in case of partial cancel
                model.SettledDate = NewTradeTransaction.SettledDate;
                model.SettledQty = (NewTradeTransaction.TrnType == 4) ? NewTradeTransaction.SettledBuyQty : NewTradeTransaction.SettledSellQty;
                model.SettlementPrice = _frontTrnRepository.GetTradeSettlementPriceMargin(NewTradeTransaction.TrnNo).SettlementPrice;
                model.ChargeRs = Convert.ToDecimal(Newtransaction.ChargeRs == null ? 0 : Newtransaction.ChargeRs);
                model.Chargecurrency = string.IsNullOrEmpty(Newtransaction.ChargeCurrency) ? "" : Newtransaction.ChargeCurrency;
                RecentOrder(model, Token, UserID, 1);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog("GetAndSendRecentOrderDataMargin ##TrnNo:" + NewTradeTransaction.TrnNo, ControllerName, ex);
                //throw ex;
            }
        }

        public string GetTokenByUserID(string ID)
        {
            try
            {
                var Redis = new RadisServices<ConnetedClientToken>(this._fact);
                string AccessToken = Redis.GetHashData(_configuration.GetValue<string>("SignalRKey:RedisToken") + ID.ToString(), "Token");
                return AccessToken;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                return "";
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
        #endregion

        //Komal 3-06-2019 Arbitrage Trading
        #region Pairwise Arbitrage Method

        public async Task BuyerBookArbitrage(ArbitrageBuySellViewModel Data, string Pair, string UserID, short IsMargin = 0)
        {
            try
            {
                SignalRComm<ArbitrageBuySellViewModel> CommonData = new SignalRComm<ArbitrageBuySellViewModel>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.BuyerBookArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveBuyerBookArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;
                CommonData.LP = 0;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.BuyerBookArbitrage;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                Task UserHub = _chat.BuyerBookArbitrage(SendData.Parameter, SendData.DataObj);
                Task.WaitAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task SellerBookArbitrage(ArbitrageBuySellViewModel Data, string Pair, string UserID, short IsMargin = 0)
        {
            try
            {
                SignalRComm<ArbitrageBuySellViewModel> CommonData = new SignalRComm<ArbitrageBuySellViewModel>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.SellerBookArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveSellerBookArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;
                CommonData.LP = 0;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.SellerBookArbitrage;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                Task UserHub = _chat.SellerBookArbitrage(SendData.Parameter, SendData.DataObj);
                Task.WaitAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task OrderHistoryArbitrage(GetTradeHistoryInfoArbitrageV1 Data, string Pair, string UserID, short IsMargin = 0)
        {
            try
            {
                SignalRComm<GetTradeHistoryInfoArbitrageV1> CommonData = new SignalRComm<GetTradeHistoryInfoArbitrageV1>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.OrderHistoryArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveOrderHistoryArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.OrderHistoryArbitrage;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                Task UserHub = _chat.OrderHistoryArbitrage(SendData.Parameter, SendData.DataObj);
                Task.WaitAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task MarketDataArbitrage(MarketCapData Data, string Pair, string UserID, short IsMargin = 0)
        {
            try
            {
                SignalRComm<MarketCapData> CommonData = new SignalRComm<MarketCapData>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.MarketDataArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveMarketDataArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.MarketDataArbitrage;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                Task UserHub = _chat.MarketDataArbitrage(SendData.Parameter, SendData.DataObj);
                Task.WaitAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task LastPriceArbitrage(LastPriceViewModelArbitrage Data, string Pair, string UserID, short IsMargin = 0)
        {
            try
            {
                SignalRComm<LastPriceViewModelArbitrage> CommonData = new SignalRComm<LastPriceViewModelArbitrage>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.PriceArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveLastPriceArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.PriceArbitrage;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                Task UserHub = _chat.LastPriceArbitrage(SendData.Parameter, SendData.DataObj);
                Task.WaitAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task StopLimitBuyerBookArbitrage(List<StopLimitBuySellBook> Data, string Pair, string UserID, short IsMargin = 0)
        {
            try
            {
                SignalRComm<List<StopLimitBuySellBook>> CommonData = new SignalRComm<List<StopLimitBuySellBook>>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.StopLimitBuyerBookArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveStopLimitBuyerBookArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.StopLimitBuyerBookArbitrage;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                Task UserHub = _chat.StopLimitBuyerBookArbitrage(SendData.Parameter, SendData.DataObj);
                Task.WaitAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task StopLimitSellerBookArbitrage(List<StopLimitBuySellBook> Data, string Pair, string UserID, short IsMargin = 0)
        {
            try
            {
                SignalRComm<List<StopLimitBuySellBook>> CommonData = new SignalRComm<List<StopLimitBuySellBook>>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.StopLimitSellerBookArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveStopLimitSellerBookArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.StopLimitSellerBookArbitrage;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                Task UserHub = _chat.StopLimitSellerBookArbitrage(SendData.Parameter, SendData.DataObj);
                Task.WaitAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task ChartDataArbitrage(GetGraphDetailInfo Data, string Pair, short IsMargin = 0)
        {
            try
            {
                SignalRComm<GetGraphDetailInfo> CommonData = new SignalRComm<GetGraphDetailInfo>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.ChartDataArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveChartDataArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.ChartDataArbitrage;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                Task UserHub = _chat.ChartDataArbitrage(SendData.Parameter, SendData.DataObj);
                Task.WaitAll();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task ChartDataEveryLastMinArbitrage(DateTime DateTime, short IsMargin = 0)
        {
            try
            {
                List<GetGraphResponsePairWise> GraphResponsesList = new List<GetGraphResponsePairWise>();
                GraphResponsesList = _frontTrnRepository.GetGraphDataEveryLastMinArbitrage(DateTime.ToString("yyyy-MM-dd HH:mm:00:000"));

                if (GraphResponsesList != null)
                {
                    foreach (GetGraphResponsePairWise GraphData in GraphResponsesList)
                    {
                        GetGraphDetailInfo GraphDetailInfo = new GetGraphDetailInfo();
                        GraphDetailInfo.Close = GraphData.CloseVal;
                        GraphDetailInfo.High = GraphData.High;
                        GraphDetailInfo.Open = GraphData.OpenVal;
                        GraphDetailInfo.Low = GraphData.Low;
                        DateTime dt2 = new DateTime(1970, 1, 1);
                        //GraphDetailInfo.DataDate = Convert.ToInt64(GraphData.DataDate.Subtract(dt2).TotalMilliseconds);
                        GraphDetailInfo.DataDate = GraphData.DataDate;
                        GraphDetailInfo.Volume = GraphData.Volume;
                        GraphDetailInfo.Close = GraphData.CloseVal;
                        ChartDataArbitrage(GraphDetailInfo, GraphData.PairName, IsMargin);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task ProviderMarketDataArbitrage(ExchangeProviderListArbitrage Data, string Pair)
        {
            try
            {
                SignalRComm<ExchangeProviderListArbitrage> CommonData = new SignalRComm<ExchangeProviderListArbitrage>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.ProviderMarketDataArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveProviderMarketDataArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.ProviderMarketDataArbitrage;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                Task UserHub = _chat.ProviderMarketDataArbitrage(SendData.Parameter, SendData.DataObj);
                Task.WaitAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task ProfitIndicatorArbitrage(ProfitIndicatorInfo Data, string Pair)
        {
            try
            {
                SignalRComm<ProfitIndicatorInfo> CommonData = new SignalRComm<ProfitIndicatorInfo>();


                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.ProfitIndicatorArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveProfitIndicatorArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.ProfitIndicatorArbitrage;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                Task UserHub = _chat.ProfitIndicatorArbitrage(SendData.Parameter, SendData.DataObj);
                Task.WaitAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task ExchangeListSmartArbitrage(List<ExchangeListSmartArbitrage> Data, string Pair)
        {
            try
            {
                SignalRComm<List<ExchangeListSmartArbitrage>> CommonData = new SignalRComm<List<ExchangeListSmartArbitrage>>();


                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.ExchangeListSmartArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveExchangeListSmartArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = Data;
                CommonData.Parameter = Pair;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.ExchangeListSmartArbitrage;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                Task UserHub = _chat.ExchangeListSmartArbitrage(SendData.Parameter, SendData.DataObj);
                Task.WaitAll();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        #endregion

        #region Arbitrage UserSpecific

        public async Task ActiveOrderArbitrage(ActiveOrderInfoArbitrageV1 Data, string Token, string UserID, short IsMargin = 0)
        {
            try
            {
                SignalRComm<ActiveOrderInfoArbitrageV1> CommonData = new SignalRComm<ActiveOrderInfoArbitrageV1>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.ActiveOrderArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveActiveOrderArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                CommonData.Data = Data;
                CommonData.Parameter = null;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.ActiveOrderArbitrage;
                SendData.Parameter = Token;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                var UserHub = _chat.ActiveOrderArbitrage(SendData.Parameter, SendData.DataObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task TradeHistoryArbitrage(GetTradeHistoryInfoArbitrageV1 Data, string Pair, string Token, string UserID, short IsMargin = 0)
        {
            try
            {
                SignalRComm<GetTradeHistoryInfoArbitrageV1> CommonData = new SignalRComm<GetTradeHistoryInfoArbitrageV1>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.TradeHistoryArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveTradeHistoryArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                CommonData.Data = Data;
                CommonData.Parameter = null;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.TradeHistoryArbitrage;
                SendData.Parameter = Token;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                var UserHub = _chat.TradeHistoryArbitrage(SendData.Parameter, SendData.DataObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task RecentOrderArbitrage(RecentOrderInfoArbitrageV1 Data, string Token, string UserID, short IsMargin = 0)
        {
            try
            {
                SignalRComm<RecentOrderInfoArbitrageV1> CommonData = new SignalRComm<RecentOrderInfoArbitrageV1>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.RecentOrderArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveRecentOrderArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                CommonData.Data = Data;
                CommonData.Parameter = null;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.RecentOrderArbitrage;
                SendData.Parameter = Token;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                var UserHub = _chat.RecentOrderArbitrage(SendData.Parameter, SendData.DataObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task WalletBalUpdateArbitrage(WalletMasterResponse Data, string Wallet, string Token, string UserID, short IsMargin = 0)
        {
            try
            {
                SignalRComm<WalletMasterResponse> CommonData = new SignalRComm<WalletMasterResponse>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.WalletBalArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveWalletBalArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                CommonData.Data = Data;
                CommonData.Parameter = null;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.WalletBalArbitrage;
                SendData.Parameter = Token;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                SendData.WalletName = Wallet;

                var UserHub = _chat.WalletBalUpdateArbitrage(SendData.Parameter, SendData.WalletName, SendData.DataObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task ActivityNotificationV2Arbitrage(ActivityNotificationMessage Notification, string Token, string UserID, short IsMargin = 0)
        {
            try
            {
                SignalRComm<ActivityNotificationMessage> CommonData = new SignalRComm<ActivityNotificationMessage>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.ActivityNotificationArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveNotificationArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                CommonData.Data = Notification;
                CommonData.Parameter = null;
                CommonData.IsMargin = IsMargin;

                //SignalRDataNotify SendData = new SignalRDataNotify();
                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.ActivityNotificationArbitrage;
                SendData.Parameter = Token;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                var UserHub = _chat.ActivityNotificationArbitrage(SendData.Parameter, SendData.DataObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task ActivityListArbitrage(ListAddWalletRequest Request, string Token, string UserID, short IsMargin = 0)
        {
            try
            {
                SignalRComm<ListAddWalletRequest> CommonData = new SignalRComm<ListAddWalletRequest>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.WalletActivityArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveWalletActivityArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                CommonData.Data = Request;
                CommonData.Parameter = null;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.WalletActivityArbitrage;
                SendData.Parameter = Token;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                var UserHub = _chat.WalletActivityArbitrage(SendData.Parameter, SendData.DataObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        #endregion

        #region BaseMarket
        public async Task PairDataArbitrage(VolumeDataRespose Data, string Base, string UserID, short IsMargin = 0)
        {
            try
            {
                SignalRComm<VolumeDataRespose> CommonData = new SignalRComm<VolumeDataRespose>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.PairDataArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecievePairDataArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.Base);
                CommonData.Data = Data;
                CommonData.Parameter = Base;
                CommonData.IsMargin = IsMargin;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.PairDataArbitrage;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                Task UserHub = _chat.PairDataArbitrage(SendData.Parameter, SendData.DataObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task MarketTickerArbitrage(List<VolumeDataRespose> Data, string UserID, string Base = "", short IsMargin = 0)
        {
            try
            {
                SignalRComm<List<VolumeDataRespose>> CommonData = new SignalRComm<List<VolumeDataRespose>>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.MarketTickerArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveMarketTickerArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.Base);
                CommonData.Data = Data;
                CommonData.Parameter = Base;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.MarketTickerArbitrage;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                Task UserHub = _chat.MarketTickerArbitrage(SendData.Parameter, SendData.DataObj);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }
        #endregion

        #region Arbitrage GlobalEvents

        public async Task OnStatusSuccessArbitrage(short Status, TransactionQueueArbitrage Newtransaction, TradeTransactionQueueArbitrage NewTradeTransaction, string Token, short OrderType, decimal SettlementPrice)
        {
            //update Recent Order
            //pop OpenOrder
            //add tradehistory
            //add orderhistory
            //buyer/seller book;
            //DateTime curtime = DateTime.UtcNow;
            string UserID = NewTradeTransaction.MemberID.ToString();
            try
            {
                GetTradeHistoryInfoArbitrageV1 historyInfo = new GetTradeHistoryInfoArbitrageV1();
                GetTradeHistoryInfoArbitrageV1 OrderhistoryInfo = new GetTradeHistoryInfoArbitrageV1();
                ArbitrageBuySellViewModel BuySellmodel;

                Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusSuccessArbitrage" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "0 start Socket call       TRNNO : " + Newtransaction.Id));
                if (string.IsNullOrEmpty(Token))
                {
                    Token = GetTokenByUserID(NewTradeTransaction.MemberID.ToString());
                }
                if (!string.IsNullOrEmpty(Token))
                {
                    historyInfo = GetAndSendTradeHistoryInfoDataArbitrage(Newtransaction, NewTradeTransaction, OrderType, 0, SettlementPrice);
                    OrderhistoryInfo = GetAndOrderHistoryInfoDataArbitrage(Newtransaction, NewTradeTransaction, OrderType, 0, SettlementPrice);
                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                    notification.MsgCode = Convert.ToInt32(enErrorCode.SignalRTrnSuccessfullySettled);
                    notification.Param1 = historyInfo.Price.ToString();
                    notification.Param2 = historyInfo.Amount.ToString();
                    notification.Param3 = historyInfo.Total.ToString();
                    notification.Type = Convert.ToInt16(EnNotificationType.Success);
                    //ActivityNotificationV2(notification, Token);

                    if (OrderType == 3)
                    {
                        Task.Run(() => Parallel.Invoke(() => GetAndSendRecentOrderDataArbitrage(Newtransaction, NewTradeTransaction, Token, OrderType, UserID).Wait(),//rita 19-7-19 added wait
                                    //  () => OrderHistoryArbitrage(historyInfo, historyInfo.PairName, NewTradeTransaction.MemberID.ToString()),
                                    () => TradeHistoryArbitrage(historyInfo, NewTradeTransaction.PairName, Token, UserID),
                                    () => ActivityNotificationV2Arbitrage(notification, Token, UserID))).Wait();//rita 19-7-19 added wait

                        //komal 10-06-2019 Send Local Trade Only
                        if (NewTradeTransaction.IsAPITrade == 0)
                            OrderHistoryArbitrage(OrderhistoryInfo, OrderhistoryInfo.PairName, NewTradeTransaction.MemberID.ToString());
                    }
                    else
                    {
                        Task.Run(() => Parallel.Invoke(() => GetAndSendRecentOrderDataArbitrage(Newtransaction, NewTradeTransaction, Token, OrderType, UserID).Wait(),//rita 19-7-19 added wait
                                   () => GetAndSendActiveOrderDataArbitrage(Newtransaction, NewTradeTransaction, Token, OrderType, UserID, 1),
                                   //() => OrderHistoryArbitrage(historyInfo, historyInfo.PairName, NewTradeTransaction.MemberID.ToString()),
                                   () => TradeHistoryArbitrage(historyInfo, NewTradeTransaction.PairName, Token, UserID),
                                   () => ActivityNotificationV2Arbitrage(notification, Token, UserID))).Wait();//rita 19-7-19 added wait

                        //komal 10-06-2019 Send Local Trade Only
                        if (NewTradeTransaction.IsAPITrade == 0)
                            OrderHistoryArbitrage(OrderhistoryInfo, OrderhistoryInfo.PairName, NewTradeTransaction.MemberID.ToString());
                    }
                    Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusSuccessArbitrage" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "2 Complete Socket call    TRNNO : " + Newtransaction.Id));

                }
                else
                    Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusSuccessArbitrage" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, " Token Not Found TRNNO : " + Newtransaction.Id + "  MemberID : " + NewTradeTransaction.MemberID.ToString()));
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //throw ex;
            }
        }

        public async Task OnStatusPartialSuccessArbitrage(short Status, TransactionQueueArbitrage Newtransaction, TradeTransactionQueueArbitrage NewTradeTransaction, string Token, short OrderType)
        {
            //update Buyer/seller book
            string UserID = NewTradeTransaction.MemberID.ToString();
            ArbitrageBuySellViewModel BuySellmodel;
            try
            {

                if (string.IsNullOrEmpty(Token))
                {
                    Token = GetTokenByUserID(NewTradeTransaction.MemberID.ToString());
                }
                if (!string.IsNullOrEmpty(Token))
                {
                    //Rita 13-3-19 for Settled Qty update
                    if (OrderType != 3)//for market ordre not sent open and recent ordre 
                    {

                        Task.Run(() => Parallel.Invoke(() => GetAndSendActiveOrderDataArbitrage(Newtransaction, NewTradeTransaction, Token, OrderType, UserID),
                                   () => GetAndSendRecentOrderDataArbitrage(Newtransaction, NewTradeTransaction, Token, OrderType, UserID).Wait())).Wait();//rita 19-7-19 added wait
                    }
                }
                else
                    Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusPartialSuccessArbitrage" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, " Token Not Found TRNNO : " + Newtransaction.Id + "  MemberID : " + NewTradeTransaction.MemberID.ToString()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task OnStatusHoldArbitrage(short Status, TransactionQueueArbitrage Newtransaction, TradeTransactionQueueArbitrage NewTradeTransaction, string Token, short OrderType)
        {
            //add OpenOrder
            //add recent order
            string UserID = NewTradeTransaction.MemberID.ToString();
            try
            {
                ArbitrageBuySellViewModel BuySellmodel;
                Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusHoldArbitrage" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "0 start Socket call       TRNNO : " + Newtransaction.Id));

                if (string.IsNullOrEmpty(Token))
                {
                    Token = GetTokenByUserID(NewTradeTransaction.MemberID.ToString());
                }
                if (!string.IsNullOrEmpty(Token))
                {
                    if (OrderType == 4)
                    {
                        HelperForLog.WriteLogForSocket("OnStatusHoldArbitrage" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, " Order type 4 call OnLtpVhange    TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName);
                        this.OnLtpChangeArbitrage(0, NewTradeTransaction.PairID, NewTradeTransaction.PairName, UserID: NewTradeTransaction.MemberID.ToString(), IsCancel: 1);
                    }
                    if (OrderType != 3)
                    {
                        Task.Run(() => Parallel.Invoke(() => GetAndSendActiveOrderDataArbitrage(Newtransaction, NewTradeTransaction, Token, OrderType, UserID),
                           () => GetAndSendRecentOrderDataArbitrage(Newtransaction, NewTradeTransaction, Token, OrderType, UserID).Wait())).Wait();//rita 19-7-19 added wait

                    }

                    Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusHoldArbitrage" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "2 Complete Socket call    TRNNO : " + Newtransaction.Id));
                }
                else
                    Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusHoldArbitrage" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, " Token Not Found TRNNO : " + Newtransaction.Id + "  MemberID : " + NewTradeTransaction.MemberID.ToString()));
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                ////throw ex;
            }
        }

        public async Task OnStatusCancelArbitrage(short Status, TransactionQueueArbitrage Newtransaction, TradeTransactionQueueArbitrage NewTradeTransaction, string Token, short OrderType, short IsPartialCancel = 0)
        {
            //pop from OpenOrder
            //update Recent order

            string UserID = NewTradeTransaction.MemberID.ToString();
            try
            {
                GetTradeHistoryInfoArbitrageV1 historyInfo = new GetTradeHistoryInfoArbitrageV1();
                GetTradeHistoryInfoArbitrageV1 OrderhistoryInfo = new GetTradeHistoryInfoArbitrageV1();
                Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusCancelArbitrage " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "0 start Socket call       TRNNO : " + Newtransaction.Id));

                if (string.IsNullOrEmpty(Token))
                {
                    Token = GetTokenByUserID(NewTradeTransaction.MemberID.ToString());
                }
                if (!string.IsNullOrEmpty(Token))
                {
                    if (OrderType == 4)
                    {
                        HelperForLog.WriteLogForSocket("OnStatusCancelArbitrage" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "1 Order type 4 call OnLtpVhange    TRNNO : " + Newtransaction.Id + " Pair :" + NewTradeTransaction.PairName);
                        this.OnLtpChangeArbitrage(0, NewTradeTransaction.PairID, NewTradeTransaction.PairName, IsCancel: 1, UserID: NewTradeTransaction.MemberID.ToString());
                    }
                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                    notification.MsgCode = Convert.ToInt32(enErrorCode.SignalRCancelOrder);
                    notification.Param1 = NewTradeTransaction.TrnNo.ToString();
                    notification.Type = Convert.ToInt16(EnNotificationType.Success);//rita 06-12-18 change from fail to success
                    if (IsPartialCancel == 0)//Fully Cancel
                    {
                        if (OrderType == 3) //for spot no open/recent order
                        {
                            Task.Run(() => Parallel.Invoke(() => ActivityNotificationV2Arbitrage(notification, Token, UserID),
                                () => GetAndSendRecentOrderDataArbitrage(Newtransaction, NewTradeTransaction, Token, OrderType, UserID).Wait())).Wait();//rita 19-7-19 added wait
                            Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusCancelArbitrage" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "2 Complete Socket call    TRNNO : " + Newtransaction.Id));
                        }
                        else
                        {
                            //komal 17-06-2019 add Trade History call 
                            historyInfo = GetAndSendTradeHistoryInfoDataArbitrage(Newtransaction, NewTradeTransaction, OrderType);

                            Task.Run(() => Parallel.Invoke(() => GetAndSendActiveOrderDataArbitrage(Newtransaction, NewTradeTransaction, Token, OrderType, UserID, 1),
                                       () => GetAndSendRecentOrderDataArbitrage(Newtransaction, NewTradeTransaction, Token, OrderType, UserID).Wait(),//rita 19-7-19 added wait
                                       () => ActivityNotificationV2Arbitrage(notification, Token, UserID),
                                       () => TradeHistoryArbitrage(historyInfo, NewTradeTransaction.PairName, Token, UserID))).Wait();//rita 19-7-19 added wait
                            Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusCancelArbitrage" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "2 Complete Socket call    TRNNO : " + Newtransaction.Id));
                        }

                    }
                    else if (IsPartialCancel == 1)//Partial Cancel
                    {
                        //komal 17-06-2019 add Trade History call 
                        historyInfo = GetAndSendTradeHistoryInfoDataArbitrage(Newtransaction, NewTradeTransaction, OrderType);
                        OrderhistoryInfo = GetAndOrderHistoryInfoDataArbitrage(Newtransaction, NewTradeTransaction, OrderType);
                        Task.Run(() => Parallel.Invoke(() => GetAndSendActiveOrderDataArbitrage(Newtransaction, NewTradeTransaction, Token, OrderType, UserID, 1),
                                        () => GetAndSendRecentOrderDataArbitrage(Newtransaction, NewTradeTransaction, Token, OrderType, UserID).Wait(),//rita 19-7-19 added wait
                                                                                                                                                // () => OrderHistoryArbitrage(historyInfo, Token, NewTradeTransaction.MemberID.ToString()),
                                        () => TradeHistoryArbitrage(historyInfo, NewTradeTransaction.PairName, Token, UserID),
                                        () => ActivityNotificationV2Arbitrage(notification, Token, UserID))).Wait();//rita 19-7-19 added wait
                        //komal 10-06-2019 Send Local Trade Only
                        if (NewTradeTransaction.IsAPITrade == 0)
                            OrderHistoryArbitrage(OrderhistoryInfo, OrderhistoryInfo.PairName, NewTradeTransaction.MemberID.ToString());
                        Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusCancelArbitrage  Fully+Cancel+Process" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "2 Complete Socket call    TRNNO : " + Newtransaction.Id));

                    }
                }
                else
                    Task.Run(() => HelperForLog.WriteLogForSocket("OnStatusCancelArbitrage" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, " Token Not Found TRNNO : " + Newtransaction.Id + "  MemberID : " + NewTradeTransaction.MemberID.ToString()));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task OnVolumeChangeArbitrage(VolumeDataRespose volumeData, MarketCapData capData, string UserID)
        {
            try
            {
                HelperForLog.WriteLogForSocket("OnVolumeChangeArbitrage" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "Call OnVolumeChangeMethod : volumeData : " + JsonConvert.SerializeObject(volumeData) + " : Market Data : " + JsonConvert.SerializeObject(capData));
                if (volumeData != null && capData != null)
                {
                    LastPriceViewModelArbitrage lastPriceData = new LastPriceViewModelArbitrage();
                    lastPriceData.LastPrice = capData.LastPrice;
                    lastPriceData.UpDownBit = volumeData.UpDownBit;

                    string Base = volumeData.PairName.Split("_")[1];

                    Task.Run(() => Parallel.Invoke(() => PairDataArbitrage(volumeData, Base, UserID, IsMargin: 1),
                                    () => MarketDataArbitrage(capData, volumeData.PairName, UserID, IsMargin: 1),
                                    () => LastPriceArbitrage(lastPriceData, volumeData.PairName, UserID, IsMargin: 1),
                                    () => HelperForLog.WriteLogForSocket("OnVolumeChangeArbitrage" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, "After Last price Call Pair :" + volumeData.PairName + "  DATA :" + JsonConvert.SerializeObject(lastPriceData))));

                    //komal 11-06-2019 LTP change call for buy-sell book
                    ArbitrageBuySellViewModel BuySellmodel = new ArbitrageBuySellViewModel();
                    BuySellmodel.LPType = (short)enAppType.COINTTRADINGLocal;
                    BuySellmodel.LTP = capData.LastPrice;
                    BuySellmodel.ProviderName = _ITrnMasterConfiguration.GetServiceProviderMasterArbitrageList().ToList().Find(e => e.Id == 2000002).ProviderName;
                    Task.Run(() => SellerBookArbitrage(BuySellmodel, volumeData.PairName, ""));
                    Task.Run(() => BuyerBookArbitrage(BuySellmodel, volumeData.PairName, ""));

                    ExchangeProviderListArbitrage exchangeProvider = new ExchangeProviderListArbitrage();
                    exchangeProvider.LPType = (short)enAppType.COINTTRADINGLocal;
                    exchangeProvider.LTP = capData.LastPrice;
                    exchangeProvider.ProviderName = _ITrnMasterConfiguration.GetServiceProviderMasterArbitrageList().ToList().Find(e => e.Id == 2000002).ProviderName; ;
                    exchangeProvider.UpDownBit = volumeData.UpDownBit;
                    exchangeProvider.Volume = volumeData.Low24Hr;
                    exchangeProvider.ChangePer = volumeData.ChangePer;
                    ProviderMarketDataArbitrage(exchangeProvider, volumeData.PairName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:OnVolumeChangeArbitrage" + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                //throw ex;
            }
        }

        public async Task OnLtpChangeArbitrage(Decimal LTP, long Pair, string PairName, short IsCancel = 0, short IsMargin = 0, string UserID = "")
        {
            List<StopLimitBuySellBook> DataBuy = new List<StopLimitBuySellBook>();
            List<StopLimitBuySellBook> DataSell = new List<StopLimitBuySellBook>();
            try
            {
                HelperForLog.WriteLogForSocket("OnLtpChange" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, " LTP :" + LTP + " Pair :" + Pair);
                if (IsCancel == 0)
                {
                    DataBuy = _frontTrnRepository.GetStopLimitBuySellBooksArbitrage(LTP, Pair, enOrderType.BuyOrder);
                    DataSell = _frontTrnRepository.GetStopLimitBuySellBooksArbitrage(LTP, Pair, enOrderType.SellOrder);
                }
                else if (IsCancel == 1)
                {
                    DataBuy = _frontTrnRepository.GetStopLimitBuySellBooksArbitrage(LTP, Pair, enOrderType.BuyOrder, 1);
                    DataSell = _frontTrnRepository.GetStopLimitBuySellBooksArbitrage(LTP, Pair, enOrderType.SellOrder, 1);
                }

                Task.Run(() => StopLimitBuyerBookArbitrage(DataBuy, PairName, UserID, IsMargin));
                Task.Run(() => StopLimitSellerBookArbitrage(DataSell, PairName, UserID, IsMargin));
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }

        public async Task SendActivityNotificationV2Arbitrage(ActivityNotificationMessage ActivityNotification, string Token, short TokenType = 1, string TrnNo = "", short IsMargin = 0)
        {
            try
            {
                var MemberID = Token;
                //HelperForLog.WriteLogForSocket("SendActivityNotificationV2Arbitrage " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, " 1 TRNNO : " + TrnNo.ToString() + "   Data : " + JsonConvert.SerializeObject(ActivityNotification) + " \n Token :" + Token);
                if (TokenType == Convert.ToInt16(enTokenType.ByUserID))
                {
                    Token = GetTokenByUserID(Token);
                }
                if (!string.IsNullOrEmpty(Token))
                {
                    ActivityNotificationV2Arbitrage(ActivityNotification, Token, MemberID, IsMargin);
                    Task.Run(() => HelperForLog.WriteLogForSocket("SendActivityNotificationV2Arbitrage " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, " 2 TRNNO : " + TrnNo.ToString() + "   Data : " + JsonConvert.SerializeObject(ActivityNotification) + " \n Token :" + Token));
                }
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //throw ex;
            }
        }

        public async Task OnWalletBalChangeArbitrage(WalletMasterResponse Data, string WalletTypeName, string Token, short TokenType = 1, string TrnNo = "", short IsMargin = 0) //2019-6-24
        {
            try
            {
                var MemberID = Token;
                if (TokenType == Convert.ToInt16(enTokenType.ByUserID))
                {
                    Token = GetTokenByUserID(Token);
                }
                if (!string.IsNullOrEmpty(Token))
                {
                    Task.Run(() => Parallel.Invoke(() => WalletBalUpdateArbitrage(Data, WalletTypeName, Token, MemberID, IsMargin),
                                    () => HelperForLog.WriteLogForSocket("OnWalletBalChangeArbitrage" + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss.ffff"), ControllerName, " Wallet Name : " + WalletTypeName + "         TRNNO : " + TrnNo.ToString() + " Member ID :" + MemberID + "   Data : " + JsonConvert.SerializeObject(Data) + " \n Token :" + Token)));
                }

            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
            }
        }
        #endregion

        #region Arbitrage HelperMethods

        public async Task GetAndSendActiveOrderDataArbitrage(TransactionQueueArbitrage Newtransaction, TradeTransactionQueueArbitrage NewTradeTransaction, string Token, short OrderType, string UserID, short IsPop = 0)
        {
            try
            {
                ActiveOrderInfoArbitrageV1 activeOrder = new ActiveOrderInfoArbitrageV1();
                activeOrder.GUID= Newtransaction.GUID.ToString();
                activeOrder.Id = Newtransaction.Id.ToString();
                activeOrder.TrnDate = Newtransaction.TrnDate;
                activeOrder.Type = (NewTradeTransaction.TrnType == 4) ? "BUY" : "SELL";
                activeOrder.Order_Currency = NewTradeTransaction.Order_Currency;
                activeOrder.Delivery_Currency = NewTradeTransaction.Delivery_Currency;
                if (IsPop == 1)
                    activeOrder.Amount = 0;
                else
                    activeOrder.Amount = (NewTradeTransaction.BuyQty == 0) ? NewTradeTransaction.SellQty : (NewTradeTransaction.SellQty == 0) ? NewTradeTransaction.BuyQty : NewTradeTransaction.BuyQty;
                activeOrder.Price = (NewTradeTransaction.BidPrice == 0) ? NewTradeTransaction.AskPrice : (NewTradeTransaction.AskPrice == 0) ? NewTradeTransaction.BidPrice : NewTradeTransaction.BidPrice;
                activeOrder.IsCancelled = NewTradeTransaction.IsCancelled;
                activeOrder.OrderType = Enum.GetName(typeof(enTransactionMarketType), OrderType);
                activeOrder.PairId = NewTradeTransaction.PairID;
                activeOrder.PairName = NewTradeTransaction.PairName;
                //Rita 12-3-19 this required for front side
                activeOrder.SettledQty = (NewTradeTransaction.TrnType == 4) ? NewTradeTransaction.SettledBuyQty : NewTradeTransaction.SettledSellQty;
                activeOrder.SettledDate = NewTradeTransaction.SettledDate;
                activeOrder.ChargeRs = Convert.ToDecimal(Newtransaction.ChargeRs);
                activeOrder.Chargecurrency = string.IsNullOrEmpty(Newtransaction.ChargeCurrency) ? "" : Newtransaction.ChargeCurrency;
                //if (Newtransaction.SerProID == 0) //08-06-2019 komal add Exchange name
                //    activeOrder.ExchangeName = "LOCAL";
                //else
                activeOrder.ExchangeName = _ITrnMasterConfiguration.GetAppTypes().ToList().Find(e => e.Id == Newtransaction.LPType).AppTypeName;

                //HelperForLog.WriteLogForSocket("GetAndSendActiveOrderDataArbitrage", ControllerName, " 1 OpenOrder call TRNNO:" + Newtransaction.Id);
                if (IsPop != 1)//send notification,not pop call
                {
                    ActivityNotificationMessage notification = new ActivityNotificationMessage();
                    notification.MsgCode = Convert.ToInt32(enErrorCode.SignalRTrnSuccessfullyCreated);
                    notification.Param1 = activeOrder.Price.ToString();
                    notification.Param2 = activeOrder.Amount.ToString();
                    notification.Type = Convert.ToInt16(EnNotificationType.Success);
                    //komal 11-11-2019 12:12 PM remove unwanted alert
                    Task.Run(() =>
                        Parallel.Invoke(() => ActiveOrderArbitrage(activeOrder, Token, UserID)
                                    //() => ActivityNotificationV2Arbitrage(notification, Token, UserID)
                                    ));
                }
                else
                {
                    ActiveOrderArbitrage(activeOrder, Token, UserID);
                }
                HelperForLog.WriteLogForSocket("GetAndSendActiveOrderData", ControllerName, " 1 ActiveOrder call TRNNO:" + Newtransaction.Id + " Order Type " + OrderType);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //throw ex;
            }
        }

        public GetTradeHistoryInfoArbitrageV1 GetAndSendTradeHistoryInfoDataArbitrage(TransactionQueueArbitrage Newtransaction, TradeTransactionQueueArbitrage NewTradeTransaction, short OrderType, short IsPop = 0, decimal SettlementPrice = 0)
        {
            try
            {
                GetTradeHistoryInfoArbitrageV1 model = new GetTradeHistoryInfoArbitrageV1();
                model.TrnNo = Newtransaction.Id.ToString();
                model.GUID = Newtransaction.GUID.ToString();
                model.Type = (NewTradeTransaction.TrnType == 4) ? "BUY" : "SELL";
                //Rita 09-4-09 in case of buy send settlement price for OrderHistory in front side , LTP and History Diff.price issue solved                
                model.Price = (NewTradeTransaction.TrnType == 4) ? (SettlementPrice == 0 ? NewTradeTransaction.BidPrice : SettlementPrice) : NewTradeTransaction.AskPrice;
                model.Amount = (NewTradeTransaction.TrnType == 4) ? NewTradeTransaction.BuyQty : NewTradeTransaction.SellQty; //Rita 19-11-18 May be Qty not fully sell from Pool
                //komal 30 April 2019 add charge
                model.ChargeRs = Convert.ToDecimal(Newtransaction.ChargeRs);
                model.Total = model.Type == "BUY" ? (((decimal)model.Price * model.Amount) - model.ChargeRs == null ? 0 : (decimal)model.ChargeRs) : (((decimal)model.Price * model.Amount));
                model.DateTime = Convert.ToDateTime(NewTradeTransaction.SettledDate);
                model.Status = NewTradeTransaction.Status;
                model.StatusText = Enum.GetName(typeof(enTransactionStatus), model.Status);
                model.PairName = NewTradeTransaction.PairName;
                
                model.Chargecurrency = string.IsNullOrEmpty(Newtransaction.ChargeCurrency) ? "" : Newtransaction.ChargeCurrency;
                model.IsCancel = NewTradeTransaction.IsCancelled;
                model.OrderType = Enum.GetName(typeof(enTransactionMarketType), OrderType);
                model.SettledDate = NewTradeTransaction.SettledDate;
                model.SettledQty = (NewTradeTransaction.TrnType == 4) ? NewTradeTransaction.SettledBuyQty : NewTradeTransaction.SettledSellQty;
                model.SettlementPrice = _frontTrnRepository.GetTradeSettlementPriceArbitrage(NewTradeTransaction.TrnNo).SettlementPrice;
                //if (Newtransaction.SerProID == 0) //08-06-2019 komal add Exchange name
                //    model.ExchangeName = "LOCAL";
                //else
                    model.ExchangeName = _ITrnMasterConfiguration.GetAppTypes().ToList().Find(e => e.Id == Newtransaction.LPType).AppTypeName;

                return model;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //throw ex;
                return null;
            }
        }

        public GetTradeHistoryInfoArbitrageV1 GetAndOrderHistoryInfoDataArbitrage(TransactionQueueArbitrage Newtransaction, TradeTransactionQueueArbitrage NewTradeTransaction, short OrderType, short IsPop = 0, decimal SettlementPrice = 0)
        {
            try
            {
                GetTradeHistoryInfoArbitrageV1 model = new GetTradeHistoryInfoArbitrageV1();
                model.TrnNo = Newtransaction.Id.ToString();
                model.GUID = Newtransaction.GUID.ToString();
                model.Type = (NewTradeTransaction.TrnType == 4) ? "BUY" : "SELL";

                //Rita 09-4-09 in case of buy send settlement price for OrderHistory in front side , LTP and History Diff.price issue solved
                var Price = _frontTrnRepository.GetTradeSettlementPriceArbitrage(NewTradeTransaction.TrnNo).SettlementPrice;
                model.Price = Price == null ? 0 : (Decimal)Price;
                model.Amount = (NewTradeTransaction.TrnType == 4) ? NewTradeTransaction.BuyQty : NewTradeTransaction.SellQty; //Rita 19-11-18 May be Qty not fully sell from Pool
                                                                                                                              //komal 30 April 2019 add charge
                model.ChargeRs = Convert.ToDecimal(Newtransaction.ChargeRs);
                model.Chargecurrency = string.IsNullOrEmpty(Newtransaction.ChargeCurrency) ? "" : Newtransaction.ChargeCurrency;
                model.Total = model.Type == "BUY" ? (((decimal)model.Price * model.Amount) - model.ChargeRs == null ? 0 : (decimal)model.ChargeRs) : (((decimal)model.Price * model.Amount));
                model.DateTime = Convert.ToDateTime(NewTradeTransaction.SettledDate);
                model.Status = NewTradeTransaction.Status;
                model.StatusText = Enum.GetName(typeof(enTransactionStatus), model.Status);
                model.PairName = NewTradeTransaction.PairName;
                
               
                model.IsCancel = NewTradeTransaction.IsCancelled;
                model.OrderType = Enum.GetName(typeof(enTransactionMarketType), OrderType);
                model.SettledDate = NewTradeTransaction.SettledDate;
                model.SettledQty = (NewTradeTransaction.TrnType == 4) ? NewTradeTransaction.SettledBuyQty : NewTradeTransaction.SettledSellQty;
                model.SettlementPrice = model.Price;
                //if (Newtransaction.SerProID == 0) //08-06-2019 komal add Exchange name
                //    model.ExchangeName = "LOCAL";
                //else
                    model.ExchangeName = _ITrnMasterConfiguration.GetAppTypes().ToList().Find(e => e.Id == Newtransaction.LPType).AppTypeName;

                return model;
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //throw ex;
                return null;
            }
        }

        public async Task GetAndSendRecentOrderDataArbitrage(TransactionQueueArbitrage Newtransaction, TradeTransactionQueueArbitrage NewTradeTransaction, string Token, short OrderType, string UserID, short IsPop = 0)
        {
            try
            {
                RecentOrderInfoArbitrageV1 model = new RecentOrderInfoArbitrageV1();
                model.TrnNo = Newtransaction.Id.ToString();
                model.GUID = Newtransaction.GUID.ToString();
                model.Type = (NewTradeTransaction.TrnType == 4) ? "BUY" : "SELL";
                model.Price = (NewTradeTransaction.BidPrice == 0) ? NewTradeTransaction.AskPrice : (NewTradeTransaction.AskPrice == 0) ? NewTradeTransaction.BidPrice : NewTradeTransaction.BidPrice;
                model.Qty = (NewTradeTransaction.TrnType == 4) ? NewTradeTransaction.BuyQty : NewTradeTransaction.SellQty; ;
                model.DateTime = NewTradeTransaction.TrnDate;
                model.Status = Enum.GetName(typeof(enTransactionStatus), NewTradeTransaction.Status);
                model.PairId = NewTradeTransaction.PairID;
                model.PairName = NewTradeTransaction.PairName;
                model.OrderType = Enum.GetName(typeof(enTransactionMarketType), OrderType);
                model.StatusCode = NewTradeTransaction.Status;
                model.IsCancel = NewTradeTransaction.IsCancelled;//Rita 22-3-19 added for separate status with success in case of partial cancel
                model.SettledDate = NewTradeTransaction.SettledDate;
                model.SettledQty = (NewTradeTransaction.TrnType == 4) ? NewTradeTransaction.SettledBuyQty : NewTradeTransaction.SettledSellQty;
                model.SettlementPrice = _frontTrnRepository.GetTradeSettlementPriceArbitrage(NewTradeTransaction.TrnNo).SettlementPrice;
               
                //if (Newtransaction.SerProID == 0) //08-06-2019 komal add Exchange name
                //    model.ExchangeName = "LOCAL";
                //else
                model.ExchangeName = _ITrnMasterConfiguration.GetAppTypes().ToList().Find(e => e.Id == Newtransaction.LPType).AppTypeName;

                RecentOrderArbitrage(model, Token, UserID);
                //HelperForLog.WriteLogForSocket("GetAndSendRecentOrderDataArbitrage", ControllerName, "2 RecentOrder TRNNO:" + Newtransaction.Id);
            }
            catch (Exception ex)
            {
                HelperForLog.WriteErrorLog(System.Reflection.MethodBase.GetCurrentMethod().Name, this.GetType().Name, ex);
                //throw ex;
            }
        }

        #endregion
    }

}