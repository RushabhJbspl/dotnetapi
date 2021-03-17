using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Core.ApiModels;
using CleanArchitecture.Core.ApiModels.Chat;
using CleanArchitecture.Core.Entities;
using CleanArchitecture.Core.Entities.Communication;
using CleanArchitecture.Core.Entities.Transaction;
using CleanArchitecture.Core.Entities.User;
using CleanArchitecture.Core.Enums;
using CleanArchitecture.Core.Helpers;
using CleanArchitecture.Core.Interfaces;
using CleanArchitecture.Core.Interfaces.FeedConfiguration;
using CleanArchitecture.Core.SignalR;
using CleanArchitecture.Core.ViewModels.Transaction;
using CleanArchitecture.Core.ViewModels.Wallet;
using CleanArchitecture.Infrastructure.BGTask;
using CleanArchitecture.Infrastructure.DTOClasses;
using CleanArchitecture.Infrastructure.Interfaces;
using CleanArchitecture.Infrastructure.Services;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CleanArchitecture.Web.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class SocketController : Controller
    {
        private SocketHub _chat;
        private readonly ILogger<SocketController> _logger;
        private readonly IMediator _mediator;
        //private readonly UserManager<ApplicationUser> _userManager;
        //private readonly ISignalRTestService _signalRTestService; 
        private readonly ISignalRService _signalRService;
        private readonly ISignalRQueue _signalRQueue;
        private readonly ICommonRepository<TradeTransactionQueue> _tradeTransactionQueueRepository;
        private readonly ICommonRepository<SettledTradeTransactionQueue> _settelTradeTranQueue;
        private readonly IExchangeFeedConfiguration _exchangeFeed;
        private readonly   ThirdPartyAPISignalRQueue _thirdPartyAPISignalRQueue;

        //public SocketController(ILogger<SocketController> logger, IMediator mediator, ISignalRTestService signalRTestService)
        public SocketController(ILogger<SocketController> logger, IMediator mediator, ISignalRService signalRService, ISignalRQueue signalRQueue,
            ICommonRepository<TradeTransactionQueue> tradeTransactionQueueRepository, ICommonRepository<SettledTradeTransactionQueue> settelTradeTranQueue,
            IExchangeFeedConfiguration exchangeFeed,ThirdPartyAPISignalRQueue thirdPartyAPISignalRQueue, SocketHub chat)
        {
            _logger = logger;
            _chat = chat;
            _mediator = mediator;
            _signalRService = signalRService;
            //_signalRTestService = signalRTestService;
            _signalRQueue = signalRQueue;
            _tradeTransactionQueueRepository = tradeTransactionQueueRepository;
            _settelTradeTranQueue = settelTradeTranQueue;
            _exchangeFeed = exchangeFeed;
            _thirdPartyAPISignalRQueue = thirdPartyAPISignalRQueue;
        }

        [HttpGet("BuyerBook/{Data}/{Pair}")]
        public async Task<IActionResult> BuyerBook(string Data, String Pair = "INR_BTC")
        {
            string ReciveMethod = "";
            try
            {
                GetBuySellBook model = new GetBuySellBook();
                model.Amount = Convert.ToDecimal(11.003);
                model.Price = Convert.ToDecimal(0.0000036);
                model.OrderId = new Guid("c0fae6bc-aba8-4eec-8ba9-fa23634222d6");
                model.RecordCount = 12;

                SignalRComm<GetBuySellBook> CommonData = new SignalRComm<GetBuySellBook>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.BuyerBookArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveBuyerBookArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = model;
                CommonData.Parameter = Pair;
                CommonData.LP = 0;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.BuyerBookArbitrage;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                Task UserHub = _chat.BuyerBookArbitrage(SendData.Parameter, SendData.DataObj);
                return Ok(new { ReciveMethod = ReciveMethod });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

                return Ok();
            }
        }

        [HttpGet("SellerBook/{Data}/{Pair}")]
        public async Task<IActionResult> SellerBook(string Data, String Pair="INR_BTC")
        {
            string ReciveMethod = "";
            try
            {
                GetBuySellBook model = new GetBuySellBook();
                model.Amount = Convert.ToDecimal(11.003);
                model.Price = Convert.ToDecimal(0.0000036);
                model.OrderId = new Guid("c0fae6bc-aba8-4eec-8ba9-fa23634222d6");
                model.RecordCount = 12;

                SignalRComm<GetBuySellBook> CommonData = new SignalRComm<GetBuySellBook>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.SellerBookArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveSellerBookArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = model;
                CommonData.Parameter = Pair;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.SellerBookArbitrage;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                Task UserHub = _chat.SellerBookArbitrage(SendData.Parameter, SendData.DataObj);
                return Ok(new { ReciveMethod = ReciveMethod });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

                return Ok();
            }
        }

        [HttpGet("OrderHistory/{Data}/{Pair}")]
        public async Task<IActionResult> OrderHistory(string Data, String Pair)
        {
            string ReciveMethod = "";
            try
            {
                GetTradeHistoryInfo model = new GetTradeHistoryInfo();
                model.Amount = 20;
                model.ChargeRs = 1;
                model.DateTime = DateTime.Now;
                model.IsCancel = 1;
                model.PairName = "INR_BTC";
                model.Price = 150;
                model.Status = 1;
                model.StatusText = "Success";
                model.TrnNo = 90;
                model.Type = "SELL";

                SignalRComm<GetTradeHistoryInfo> CommonData = new SignalRComm<GetTradeHistoryInfo>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.OrderHistoryArbitrage);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveOrderHistoryArbitrage);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.Broadcast);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = model;
                CommonData.Parameter = Pair;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.OrderHistoryArbitrage;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                Task UserHub = _chat.OrderHistory(SendData.Parameter, SendData.DataObj);

                return Ok(new { ReciveMethod = ReciveMethod });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

                return Ok();
            }
        }

        [HttpGet("ChartData/{Data}/{Pair}")]
        public async Task<IActionResult> ChartData(string Data, String Pair)
        {
            string ReciveMethod = "";
            try
            {
                GetGraphResponse model = new GetGraphResponse();
                //model.ChangePer = 20;
                //model.DataDate = 20180203073000;
                //model.High = 1199;
                //model.Low = 1177;
                //model.OpenVal = 1452;
                //model.TodayOpen = 1477;
                //model.Volume = 173;

                GetGraphDetailInfo temp = JsonConvert.DeserializeObject<GetGraphDetailInfo>(Data);

                SignalRComm<GetGraphDetailInfo> CommonData = new SignalRComm<GetGraphDetailInfo>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.ChartData);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveChartData);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = temp;
                CommonData.Parameter = Pair;// "INR_BTC";

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.ChartData;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                //await _mediator.Send(SendData);
                _signalRQueue.Enqueue(SendData);
                ReciveMethod = "RecieveChartData";
                return Ok(new { ReciveMethod = ReciveMethod });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

                return Ok();
            }
        }

        [HttpGet("MarketData/{Data}/{Pair}")]
        public async Task<IActionResult> MarketData(string Data, String Pair)
        {
            string ReciveMethod = "";
            try
            {
                MarketCapData model = new MarketCapData();
                model.Change24 = 1;
                model.ChangePer = 3;
                model.High24 = 1153;
                model.Low24 = 1125;
                model.LastPrice = 1137;
                model.Volume24 = 253;

                MarketCapData temp = JsonConvert.DeserializeObject<MarketCapData>(Data);

                SignalRComm<MarketCapData> CommonData = new SignalRComm<MarketCapData>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.MarketData);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveMarketData);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = temp;
                CommonData.Parameter = Pair;// "INR_BTC";

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.MarketData;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                //await _mediator.Send(SendData);
                _signalRQueue.Enqueue(SendData);
                ReciveMethod = "RecieveMarketData";
                return Ok(new { ReciveMethod = ReciveMethod });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

                return Ok();
            }
        }

        [HttpGet("LastPrice/{Data}/{Pair}")]
        public async Task<IActionResult> LastPrice(string Data, String Pair)
        {
            string ReciveMethod = "";
            try
            {
                LastPriceViewModel temp = JsonConvert.DeserializeObject<LastPriceViewModel>(Data);
                SignalRComm<LastPriceViewModel> CommonData = new SignalRComm<LastPriceViewModel>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.Price);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveLastPrice);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.PairName);
                CommonData.Data = temp;
                CommonData.Parameter = Pair;// "INR_BTC";

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.Price;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                //await _mediator.Send(SendData);
                _signalRQueue.Enqueue(SendData);
                ReciveMethod = CommonData.ReturnMethod;
                return Ok(new { ReciveMethod = ReciveMethod });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

                return Ok();
            }
        }

        [HttpGet("OpenOrder/{Data}")]
        [Authorize]
        public async Task<IActionResult> OpenOrder(string Data)
        {
            string ReciveMethod = "";
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                ActiveOrderInfo model = new ActiveOrderInfo();
                model.Id = 96;
                model.TrnDate = DateTime.UtcNow;
                model.Type = "BUY";
                model.Order_Currency = "BTC";
                model.Delivery_Currency = "LTC";
                model.Amount = 100;
                model.Price = 1400;
                model.IsCancelled = 1;
                model.PairName = "INR_BTC";
                model.PairId = 10021001;
                ActiveOrderInfo temp = JsonConvert.DeserializeObject<ActiveOrderInfo>(Data);

                SignalRComm<ActiveOrderInfo> CommonData = new SignalRComm<ActiveOrderInfo>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.ActiveOrder);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveActiveOrder);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                CommonData.Data = temp;
                CommonData.Parameter = null;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.ActiveOrder;
                SendData.Parameter = accessToken;// CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                //await _mediator.Send(SendData);
                _signalRQueue.Enqueue(SendData);
                ReciveMethod = CommonData.ReturnMethod;
                return Ok(new { ReciveMethod = ReciveMethod });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

                return Ok();
            }
        }

        [HttpGet("TradeHistory/{Data}")]
        [Authorize]
        public async Task<IActionResult> TradeHistory(string Data)
        {
            string ReciveMethod = "";
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                GetTradeHistoryInfo model = new GetTradeHistoryInfo();
                model.TrnNo = 90;
                model.Type = "SELL";
                model.Price = 1400;
                model.Amount = 1000;
                model.Total = 140000;
                model.DateTime = DateTime.UtcNow;
                model.Status = 1;
                model.StatusText = "Success";
                model.PairName = "INR_BTC";
                model.ChargeRs = 10;
                model.IsCancel = 0;

                GetTradeHistoryInfo temp = JsonConvert.DeserializeObject<GetTradeHistoryInfo>(Data);

                SignalRComm<GetTradeHistoryInfo> CommonData = new SignalRComm<GetTradeHistoryInfo>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.TradeHistory);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveTradeHistory);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                CommonData.Data = temp;
                CommonData.Parameter = null;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.TradeHistory;
                SendData.Parameter = accessToken;// CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                //await _mediator.Send(SendData);
                _signalRQueue.Enqueue(SendData);
                ReciveMethod = CommonData.ReturnMethod;
                return Ok(new { ReciveMethod = ReciveMethod });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

                return Ok();
            }
        }

        [HttpGet("RecentOrder/{Data}")]
        [Authorize]
        public async Task<IActionResult> RecentOrder(string Data)
        {
            string ReciveMethod = "";
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                RecentOrderInfo model = new RecentOrderInfo();
                //model.TrnNo = 90;
                //model.Type = "SELL";
                //model.Price = 1400;
                //model.Qty = 1000;
                //model.DateTime = DateTime.UtcNow;
                //model.Status = 1;
                //model.StatusText = "Success";
                //model.PairName = "INR_BTC";
                //model.ChargeRs = 10;

                RecentOrderInfo temp = JsonConvert.DeserializeObject<RecentOrderInfo>(Data);

                SignalRComm<RecentOrderInfo> CommonData = new SignalRComm<RecentOrderInfo>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.RecentOrder);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveRecentOrder);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                CommonData.Data = temp;
                CommonData.Parameter = null;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.RecentOrder;
                SendData.Parameter = accessToken;// CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);


                ThirdPartyAPISinalR TPSendData = new ThirdPartyAPISinalR();
                TPSendData.Method = enMethodName.RecentOrder;
                TPSendData.DataObj = JsonConvert.SerializeObject(CommonData);
                TPSendData.UserID = "57";
                TPSendData.IsPrivate = 1;
                _thirdPartyAPISignalRQueue.Enqueue(TPSendData);

                
                //await _mediator.Send(SendData);
                //_signalRQueue.Enqueue(SendData);
                ReciveMethod = CommonData.ReturnMethod;
                return Ok(new { ReciveMethod = ReciveMethod });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

                return Ok();
            }
        }

        [HttpGet("BuyerSideWalletBal/{Data}")]
        [Authorize]
        public async Task<IActionResult> BuyerSideWalletBal(string Data)
        {
            string ReciveMethod = "";
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                WalletMasterResponse model = new WalletMasterResponse();
                model.WalletName = "BTC Default";
                model.AccWalletID = "1029399266000200";
                model.Balance = 201200;
                model.CoinName = "BTC";
                model.IsDefaultWallet = 0;
                model.PublicAddress = "";
                WalletMasterResponse temp = JsonConvert.DeserializeObject<WalletMasterResponse>(Data);

                SignalRComm<WalletMasterResponse> CommonData = new SignalRComm<WalletMasterResponse>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.BuyerSideWallet);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveWalletBal);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                CommonData.Data = temp;
                CommonData.Parameter = null;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.BuyerSideWallet;
                SendData.Parameter = accessToken;// CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                SendData.WalletName = temp.CoinName;

                //await _mediator.Send(SendData);
                _signalRQueue.Enqueue(SendData);
                ReciveMethod = CommonData.ReturnMethod;
                return Ok(new { ReciveMethod = ReciveMethod });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

                return Ok();
            }
        }

        [HttpGet("SellerSideWalletBal/{Data}")]
        [Authorize]
        public async Task<IActionResult> SellerSideWalletBal(string Data)
        {
            string ReciveMethod = "";
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                WalletMasterResponse model = new WalletMasterResponse();
                model.WalletName = "LTC Default";
                model.AccWalletID = "1053841474000201";
                model.Balance = 201200;
                model.CoinName = "INR";
                model.IsDefaultWallet = 1;
                model.PublicAddress = "";

                WalletMasterResponse temp = JsonConvert.DeserializeObject<WalletMasterResponse>(Data);

                SignalRComm<WalletMasterResponse> CommonData = new SignalRComm<WalletMasterResponse>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.SellerSideWallet);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveSellerSideWalletBal);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                CommonData.Data = temp;
                CommonData.Parameter = null;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.SellerSideWallet;
                SendData.Parameter = accessToken;// CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                SendData.WalletName = temp.CoinName;
                //await _mediator.Send(SendData);
                _signalRQueue.Enqueue(SendData);
                ReciveMethod = CommonData.ReturnMethod;
                return Ok(new { ReciveMethod = ReciveMethod });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

                return Ok();
            }
        }

        [HttpGet("ActivityNotification/{Data}")]
        //[Authorize]
        public async Task<IActionResult> ActivityNotification(string Data)
        {
            string ReciveMethod = "";
            try
            {
                //var accessToken = await HttpContext.GetTokenAsync("access_token");
                //SignalRComm<String> CommonData = new SignalRComm<String>();
                //CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                //CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.ActivityNotification);
                //CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveNotification);
                //CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                //CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
                //CommonData.Data = Data;
                //CommonData.Parameter = null;

                //SignalRData SendData = new SignalRData();
                //SendData.Method = enMethodName.ActivityNotification;
                //SendData.Parameter = accessToken;
                //SendData.DataObj = JsonConvert.SerializeObject(CommonData);

                //await _mediator.Send(SendData);
                //ReciveMethod = CommonData.ReturnMethod;
                _signalRService.SendActivityNotificationV2(new ActivityNotificationMessage(), "35", 2);
                return Ok(new { ReciveMethod = ReciveMethod });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

                return Ok();
            }
        }

        [HttpGet("PairData/{Data}")]
        public async Task<IActionResult> PairData(string Data)
        {
            string ReciveMethod = "";
            try
            {
                VolumeDataRespose model = new VolumeDataRespose();
                model.ChangePer = 20;
                model.Currentrate = 1;
                model.High24Hr = 1814;
                model.High52Week = 1744;
                model.HighWeek = 1800;
                model.Low24Hr = 1812;
                model.Low52Week = 1725;
                model.LowWeek = 1700;
                model.PairId = 10021001;
                model.PairName = "INR_BTC";
                model.UpDownBit = 0;
                model.Volume24 = 1406;


                VolumeDataRespose temp = JsonConvert.DeserializeObject<VolumeDataRespose>(Data);
                string a = temp.PairName.Split("_")[1];
                SignalRComm<VolumeDataRespose> CommonData = new SignalRComm<VolumeDataRespose>();
                CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
                CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.PairData);
                CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecievePairData);
                CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
                CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.Base);
                CommonData.Data = temp;
                CommonData.Parameter = a;

                SignalRData SendData = new SignalRData();
                SendData.Method = enMethodName.PairData;
                SendData.Parameter = CommonData.Parameter;
                SendData.DataObj = JsonConvert.SerializeObject(CommonData);
                //await _mediator.Send(SendData);
                _signalRQueue.Enqueue(SendData);
                ReciveMethod = CommonData.ReturnMethod;
                return Ok(new { ReciveMethod = ReciveMethod });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

                return Ok();
            }
        }

        //[HttpGet("MarketTicker/{Data}")]
        //public async Task<IActionResult> MarketTicker(string Data)
        //{
        //    string ReciveMethod = "";
        //    try
        //    {
        //        VolumeDataRespose model = new VolumeDataRespose();
        //        model.ChangePer = 20;
        //        model.Currentrate = 1;
        //        model.High24Hr = 1814;
        //        model.High52Week = 1744;
        //        model.HighWeek = 1800;
        //        model.Low24Hr = 1812;
        //        model.Low52Week = 1725;
        //        model.LowWeek = 1700;
        //        model.PairId = 10021001;
        //        model.PairName = "INR_BTC";
        //        model.UpDownBit = 0;
        //        model.Volume24 = 1406;


        //        //VolumeDataRespose temp = JsonConvert.DeserializeObject<VolumeDataRespose>(Data);

        //        SignalRComm<VolumeDataRespose> CommonData = new SignalRComm<VolumeDataRespose>();
        //        CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
        //        CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.MarketTicker);
        //        CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveMarketTicker);
        //        CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
        //        CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.Base);
        //        CommonData.Data = model;
        //        CommonData.Parameter = "BTC";

        //        SignalRData SendData = new SignalRData();
        //        SendData.Method = enMethodName.MarketTicker;
        //        SendData.Parameter = CommonData.Parameter;
        //        SendData.DataObj = JsonConvert.SerializeObject(CommonData);
        //        await _mediator.Send(SendData);
        //        ReciveMethod = "RecieveMarketTicker";
        //        return Ok(new { ReciveMethod = ReciveMethod });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

        //        return Ok();
        //    }
        //}

        //[HttpGet("SendGroupMessage/{Data}")]
        //public async Task<IActionResult> SendGroupMessage(string Data)
        //{
        //    string ReciveMethod = "";
        //    try
        //    {

        //        SignalRComm<String> CommonData = new SignalRComm<String>();
        //        CommonData.EventType = Enum.GetName(typeof(enSignalREventType), enSignalREventType.Channel);
        //        CommonData.Method = Enum.GetName(typeof(enMethodName), enMethodName.SendGroupMessage);
        //        CommonData.ReturnMethod = Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveNotification);
        //        CommonData.Subscription = Enum.GetName(typeof(enSubscriptionType), enSubscriptionType.OneToOne);
        //        CommonData.ParamType = Enum.GetName(typeof(enSignalRParmType), enSignalRParmType.AccessToken);
        //        CommonData.Data = Data;
        //        CommonData.Parameter = null;

        //        SignalRData SendData = new SignalRData();
        //        SendData.Method = enMethodName.SendGroupMessage;
        //        SendData.Parameter = "Name :" + Data;
        //        SendData.DataObj = Data;

        //        await _mediator.Send(SendData);
        //        ReciveMethod = CommonData.ReturnMethod;
        //        return Ok(new { ReciveMethod = ReciveMethod });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

        //        return Ok();
        //    }
        //}
        [HttpGet("TestSpeed1VolumeData")]
        [Authorize]
        public async Task<IActionResult> TestSpeed1VolumeData()
        {
            string ReciveMethod = "";
            try
            {
                //var accessToken = "CfDJ8GX72IE05DhMsT4VIPEe9lrYwE8RDv2E9tXdGyjAEN-S0i2ye_m6crUern_vd3Y0kt3oXuxe9TPGPx6pfJhPnc9n9la0dtqgRyYIc-rCCudrxJ1Gnd37aDKEVg25keE_FURf7vDHxucBYUGJ0xIi8DIXfFadiuQA_Og2h5w5TY-rmzas7INqIIBQpfcAp65wH3VBsdIFGKXilBSDGlA4ZduJCilf4QQfO6ukSBSj7CGN1KGIZ0isra4HtbhROsCEVQDxpw5-EJdimyqNo9r8rvX8vqGRGqg-6uJmGSDHFZ3ifWPLeup6pmyRS5fzQvB3Ly6uIg2Rkhbp1vhKFNwtG1mRXNeGdNOB_3T05paFwD15g8U5AKJajSTC2xDtpz9pZtPEjAVUhY_udVu0zDzgR2tGBt_f0Jvupq2vzF3l9iwrBm4EvevrINGA_n_CTsYoh4qRnwuytG3kmD8HbgL7QefSWax8vOj_2AdqrtNV_RUO8ADJsayjkikwXovcgNWe5bjqExWPapei-9JlZYPRK5R1AHhh1r-Uza6UbpKw81C6mMq4EsgZ8nZ6dpNqMp4m5gwWYKimF9vGHLVLTu3azX6CZd9oAcCwbQeBq5VGYvn9xpL3PZHKeAH-zJ4OXGTsNGUnOhe9lHxgJqXMd5DrhZBUTmFHt6W8XnqRQXBfNPTYO0bDhhWTSQ_Zj-KDbrBtpBelqLavPrvR9V2G4V7tWKLjzbVOUVbLGfheA0i7YKsG";
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                
                VolumeDataRespose model = new VolumeDataRespose();
                model.ChangePer = 20;
                model.Currentrate = 1;
                model.High24Hr = 1814;
                model.High52Week = 1744;
                model.HighWeek = 1800;
                model.Low24Hr = 1812;
                model.Low52Week = 1725;
                model.LowWeek = 1700;
                model.PairId = 10021001;
                model.PairName = "INR_BTC";
                model.UpDownBit = 0;
                model.Volume24 = 1406;

                MarketCapData model2 = new MarketCapData();
                model2.Change24 = 1;
                model2.ChangePer = 3;
                model2.High24 = 1153;
                model2.Low24 = 1125;
                model2.LastPrice = 1137;
                model2.Volume24 = 253;
               // _signalRService.OnVolumeChange(model, model2);
                //_signalRService.OnVolumeChange(model, model2);
                //_signalRService.OnVolumeChange(model, model2);
                return Ok(new { ReciveMethod = ReciveMethod });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

                return Ok();
            }
        }

        [HttpGet("TrnSpeedTestOnSuccess")]
        //[Authorize]
        public async Task<IActionResult> TrnSpeedTestOnSuccess()
        {
            string ReciveMethod = "";
            try
            {
                //var accessToken = "CfDJ8GX72IE05DhMsT4VIPEe9lrYwE8RDv2E9tXdGyjAEN-S0i2ye_m6crUern_vd3Y0kt3oXuxe9TPGPx6pfJhPnc9n9la0dtqgRyYIc-rCCudrxJ1Gnd37aDKEVg25keE_FURf7vDHxucBYUGJ0xIi8DIXfFadiuQA_Og2h5w5TY-rmzas7INqIIBQpfcAp65wH3VBsdIFGKXilBSDGlA4ZduJCilf4QQfO6ukSBSj7CGN1KGIZ0isra4HtbhROsCEVQDxpw5-EJdimyqNo9r8rvX8vqGRGqg-6uJmGSDHFZ3ifWPLeup6pmyRS5fzQvB3Ly6uIg2Rkhbp1vhKFNwtG1mRXNeGdNOB_3T05paFwD15g8U5AKJajSTC2xDtpz9pZtPEjAVUhY_udVu0zDzgR2tGBt_f0Jvupq2vzF3l9iwrBm4EvevrINGA_n_CTsYoh4qRnwuytG3kmD8HbgL7QefSWax8vOj_2AdqrtNV_RUO8ADJsayjkikwXovcgNWe5bjqExWPapei-9JlZYPRK5R1AHhh1r-Uza6UbpKw81C6mMq4EsgZ8nZ6dpNqMp4m5gwWYKimF9vGHLVLTu3azX6CZd9oAcCwbQeBq5VGYvn9xpL3PZHKeAH-zJ4OXGTsNGUnOhe9lHxgJqXMd5DrhZBUTmFHt6W8XnqRQXBfNPTYO0bDhhWTSQ_Zj-KDbrBtpBelqLavPrvR9V2G4V7tWKLjzbVOUVbLGfheA0i7YKsG";
                //var accessToken = await HttpContext.GetTokenAsync("access_token");

                TransactionQueueArbitrage Newtransaction = new TransactionQueueArbitrage();
                Newtransaction.TrnType = 4;
                Newtransaction.Id = 91;
                Newtransaction.TrnDate = DateTime.Now;
                TradeTransactionQueueArbitrage NewTradeTransaction = new TradeTransactionQueueArbitrage();
                //NewTradeTransaction = _tradeTransactionQueueRepository.GetSingle(15573);
                NewTradeTransaction.Status = 4;
                NewTradeTransaction.PairName = "LTC_BTC";
                NewTradeTransaction.BidPrice = 20;
                NewTradeTransaction.PairID = 10031001;
                NewTradeTransaction.AskPrice =Convert.ToDecimal(20);
                NewTradeTransaction.TrnType = 5;
                NewTradeTransaction.Order_Currency = "BTC";
                NewTradeTransaction.Delivery_Currency = "LTC";
                NewTradeTransaction.BuyQty = 0;
                NewTradeTransaction.SellQty =Convert.ToDecimal(700.000000000000000000);
                NewTradeTransaction.IsCancelled = 0;
                NewTradeTransaction.MemberID = 28;
                NewTradeTransaction.SellQty = 1;
                NewTradeTransaction.SettledDate = DateTime.UtcNow;
                
                //Parallel.Invoke(() => _signalRService.OnStatusSuccess(1, Newtransaction, NewTradeTransaction, accessToken, 4),()=> _signalRService.OnStatusHold(4, Newtransaction, NewTradeTransaction, accessToken, 4));
                //_signalRService.OnStatusSuccess(1, Newtransaction, NewTradeTransaction, "", 1,0);
                _signalRService.OnStatusHoldArbitrage(4, Newtransaction, NewTradeTransaction, "", 1);
                //_signalRService.OnStatusSuccessArbitrage(1, Newtransaction, NewTradeTransaction, "", 1, Convert.ToDecimal(20));
                //_signalRService.OnStatusPartialSuccessArbitrage(4, Newtransaction, NewTradeTransaction, "", 1);
                //_signalRService.OnStatusCancelArbitrage(4, Newtransaction, NewTradeTransaction, "", 1);
                //Task.Run(()=>_signalRService.OnStatusHold(4, Newtransaction, NewTradeTransaction, accessToken, 4));

                return Ok(new { ReciveMethod = ReciveMethod });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

                return Ok();
            }
        }

        [HttpGet("TrnSpeedTestOnHold")]
        //[Authorize]
        public async Task<IActionResult> TrnSpeedTestOnHold()
        {
            string ReciveMethod = "";
            try
            {
                //var accessToken = "CfDJ8GX72IE05DhMsT4VIPEe9lrYwE8RDv2E9tXdGyjAEN-S0i2ye_m6crUern_vd3Y0kt3oXuxe9TPGPx6pfJhPnc9n9la0dtqgRyYIc-rCCudrxJ1Gnd37aDKEVg25keE_FURf7vDHxucBYUGJ0xIi8DIXfFadiuQA_Og2h5w5TY-rmzas7INqIIBQpfcAp65wH3VBsdIFGKXilBSDGlA4ZduJCilf4QQfO6ukSBSj7CGN1KGIZ0isra4HtbhROsCEVQDxpw5-EJdimyqNo9r8rvX8vqGRGqg-6uJmGSDHFZ3ifWPLeup6pmyRS5fzQvB3Ly6uIg2Rkhbp1vhKFNwtG1mRXNeGdNOB_3T05paFwD15g8U5AKJajSTC2xDtpz9pZtPEjAVUhY_udVu0zDzgR2tGBt_f0Jvupq2vzF3l9iwrBm4EvevrINGA_n_CTsYoh4qRnwuytG3kmD8HbgL7QefSWax8vOj_2AdqrtNV_RUO8ADJsayjkikwXovcgNWe5bjqExWPapei-9JlZYPRK5R1AHhh1r-Uza6UbpKw81C6mMq4EsgZ8nZ6dpNqMp4m5gwWYKimF9vGHLVLTu3azX6CZd9oAcCwbQeBq5VGYvn9xpL3PZHKeAH-zJ4OXGTsNGUnOhe9lHxgJqXMd5DrhZBUTmFHt6W8XnqRQXBfNPTYO0bDhhWTSQ_Zj-KDbrBtpBelqLavPrvR9V2G4V7tWKLjzbVOUVbLGfheA0i7YKsG";
                //var accessToken = await HttpContext.GetTokenAsync("access_token");

                TransactionQueue Newtransaction = new TransactionQueue();
                Newtransaction.TrnType = 4;
                Newtransaction.Id = 91;
                Newtransaction.TrnDate = DateTime.Now;
                TradeTransactionQueue NewTradeTransaction = new TradeTransactionQueue();
                NewTradeTransaction.PairName = "INR_BTC";
                NewTradeTransaction.BidPrice = Convert.ToDecimal(0.00005000);
                NewTradeTransaction.PairID = 10021001;
                NewTradeTransaction.AskPrice = 1500;
                NewTradeTransaction.TrnType = 4;
                NewTradeTransaction.Order_Currency = "BTC";
                NewTradeTransaction.Delivery_Currency = "INR";
                NewTradeTransaction.BuyQty = 10;
                NewTradeTransaction.SellQty = 0;
                NewTradeTransaction.IsCancelled = 0;
                NewTradeTransaction.MemberID = 57;
                NewTradeTransaction.TrnNo = 3377;
                //Parallel.Invoke(() => _signalRService.OnStatusSuccess(1, Newtransaction, NewTradeTransaction, accessToken, 4),()=> _signalRService.OnStatusHold(4, Newtransaction, NewTradeTransaction, accessToken, 4));
                _signalRService.OnStatusHold(4, Newtransaction, NewTradeTransaction, "", 1);
                //_signalRService.OnStatusSuccess(1, Newtransaction, NewTradeTransaction, accessToken, 4);
                //Task.Run(()=>_signalRService.OnStatusHold(4, Newtransaction, NewTradeTransaction, accessToken, 4));

                return Ok(new { ReciveMethod = ReciveMethod });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

                return Ok();
            }
        }


        [HttpGet("TrnSpeedTestOnCancel")]
        //[Authorize]
        public async Task<IActionResult> TrnSpeedTestOnCancel()
        {
            string ReciveMethod = "";
            try
            {
                //var accessToken = "CfDJ8GX72IE05DhMsT4VIPEe9lrYwE8RDv2E9tXdGyjAEN-S0i2ye_m6crUern_vd3Y0kt3oXuxe9TPGPx6pfJhPnc9n9la0dtqgRyYIc-rCCudrxJ1Gnd37aDKEVg25keE_FURf7vDHxucBYUGJ0xIi8DIXfFadiuQA_Og2h5w5TY-rmzas7INqIIBQpfcAp65wH3VBsdIFGKXilBSDGlA4ZduJCilf4QQfO6ukSBSj7CGN1KGIZ0isra4HtbhROsCEVQDxpw5-EJdimyqNo9r8rvX8vqGRGqg-6uJmGSDHFZ3ifWPLeup6pmyRS5fzQvB3Ly6uIg2Rkhbp1vhKFNwtG1mRXNeGdNOB_3T05paFwD15g8U5AKJajSTC2xDtpz9pZtPEjAVUhY_udVu0zDzgR2tGBt_f0Jvupq2vzF3l9iwrBm4EvevrINGA_n_CTsYoh4qRnwuytG3kmD8HbgL7QefSWax8vOj_2AdqrtNV_RUO8ADJsayjkikwXovcgNWe5bjqExWPapei-9JlZYPRK5R1AHhh1r-Uza6UbpKw81C6mMq4EsgZ8nZ6dpNqMp4m5gwWYKimF9vGHLVLTu3azX6CZd9oAcCwbQeBq5VGYvn9xpL3PZHKeAH-zJ4OXGTsNGUnOhe9lHxgJqXMd5DrhZBUTmFHt6W8XnqRQXBfNPTYO0bDhhWTSQ_Zj-KDbrBtpBelqLavPrvR9V2G4V7tWKLjzbVOUVbLGfheA0i7YKsG";
                //var accessToken = await HttpContext.GetTokenAsync("access_token");

                TransactionQueue Newtransaction = new TransactionQueue();
                Newtransaction.TrnType = 4;
                Newtransaction.Id = 91;
                Newtransaction.TrnDate = DateTime.Now;
                TradeTransactionQueue NewTradeTransaction = new TradeTransactionQueue();
                NewTradeTransaction.PairName = "INR_BTC";
                NewTradeTransaction.BidPrice = 1450;
                NewTradeTransaction.PairID = 10021001;
                NewTradeTransaction.AskPrice = 1500;
                NewTradeTransaction.TrnType = 4;
                NewTradeTransaction.Order_Currency = "BTC";
                NewTradeTransaction.Delivery_Currency = "INR";
                NewTradeTransaction.BuyQty = 2;
                NewTradeTransaction.SellQty = 1;
                NewTradeTransaction.IsCancelled = 0;
                NewTradeTransaction.MemberID = 57;
                //Parallel.Invoke(() => _signalRService.OnStatusSuccess(1, Newtransaction, NewTradeTransaction, accessToken, 4),()=> _signalRService.OnStatusHold(4, Newtransaction, NewTradeTransaction, accessToken, 4));
                _signalRService.OnStatusCancel(4, Newtransaction, NewTradeTransaction, "", 1);
                //_signalRService.OnStatusSuccess(1, Newtransaction, NewTradeTransaction, accessToken, 4);
                //Task.Run(()=>_signalRService.OnStatusHold(4, Newtransaction, NewTradeTransaction, accessToken, 4));

                return Ok(new { ReciveMethod = ReciveMethod });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

                return Ok();
            }
        }

        [HttpGet("TrnSpeedTestWallet")]
        [Authorize]
        public async Task<IActionResult> TrnSpeedTestWallet()
        {
            string ReciveMethod = "";
            try
            {
                //var accessToken = "CfDJ8GX72IE05DhMsT4VIPEe9lrYwE8RDv2E9tXdGyjAEN-S0i2ye_m6crUern_vd3Y0kt3oXuxe9TPGPx6pfJhPnc9n9la0dtqgRyYIc-rCCudrxJ1Gnd37aDKEVg25keE_FURf7vDHxucBYUGJ0xIi8DIXfFadiuQA_Og2h5w5TY-rmzas7INqIIBQpfcAp65wH3VBsdIFGKXilBSDGlA4ZduJCilf4QQfO6ukSBSj7CGN1KGIZ0isra4HtbhROsCEVQDxpw5-EJdimyqNo9r8rvX8vqGRGqg-6uJmGSDHFZ3ifWPLeup6pmyRS5fzQvB3Ly6uIg2Rkhbp1vhKFNwtG1mRXNeGdNOB_3T05paFwD15g8U5AKJajSTC2xDtpz9pZtPEjAVUhY_udVu0zDzgR2tGBt_f0Jvupq2vzF3l9iwrBm4EvevrINGA_n_CTsYoh4qRnwuytG3kmD8HbgL7QefSWax8vOj_2AdqrtNV_RUO8ADJsayjkikwXovcgNWe5bjqExWPapei-9JlZYPRK5R1AHhh1r-Uza6UbpKw81C6mMq4EsgZ8nZ6dpNqMp4m5gwWYKimF9vGHLVLTu3azX6CZd9oAcCwbQeBq5VGYvn9xpL3PZHKeAH-zJ4OXGTsNGUnOhe9lHxgJqXMd5DrhZBUTmFHt6W8XnqRQXBfNPTYO0bDhhWTSQ_Zj-KDbrBtpBelqLavPrvR9V2G4V7tWKLjzbVOUVbLGfheA0i7YKsG";
                var accessToken = await HttpContext.GetTokenAsync("access_token");

                WalletMasterResponse model3 = new WalletMasterResponse();
                model3.WalletName = "LTC Default";
                model3.AccWalletID = "1053841474000201";
                model3.Balance = 201200;
                model3.CoinName = "BTC";
                model3.IsDefaultWallet = 1;
                model3.PublicAddress = "";
                _signalRService.OnWalletBalChange(model3, model3.CoinName, accessToken);
                //Task.Run(() => HelperForLog.WriteLogForSocket("TrnSpeedTest3", "SocketController", "Test Task.Run();"));
                return Ok(new { ReciveMethod = ReciveMethod });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

                return Ok();
            }
        }

        [HttpGet("TrnSpeedTestNotification")]
        [Authorize]
        public async Task<IActionResult> TrnSpeedTestNotification()
        {
            string ReciveMethod = "";
            try
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                ActivityNotificationMessage notification = new ActivityNotificationMessage();
                notification.MsgCode = Convert.ToInt32(enErrorCode.SignalRTrnSuccessfullyCreated);
                notification.Type = Convert.ToInt16(EnNotificationType.Success);
                _signalRService.SendActivityNotificationV2(notification, accessToken);
                return Ok(new { ReciveMethod = ReciveMethod });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);

                return Ok();
            }
        }
        [HttpGet("ReloadSocketFeedLimit")]
        public async Task<IActionResult> ReloadSocketFeedLimit()
        {
            
            try
            {
                _exchangeFeed.ReloadFeedLimitCount();
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected exception occured,\nMethodName:" + System.Reflection.MethodBase.GetCurrentMethod().Name + "\nClassname=" + this.GetType().Name, LogLevel.Error);
                return Ok();
            }
        }

    }
}
