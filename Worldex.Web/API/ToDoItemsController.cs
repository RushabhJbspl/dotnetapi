using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Worldex.Core.ApiModels;
using Worldex.Core.ApiModels.Chat;
using Worldex.Core.Entities;
using Worldex.Core.Enums;
using Worldex.Core.Helpers;
using Worldex.Core.Interfaces;
using Worldex.Core.Interfaces.Repository;
using Worldex.Core.Services.RadisDatabase;
using Worldex.Core.ViewModels;
using Worldex.Core.ViewModels.LiquidityProvider;
using Worldex.Infrastructure.BGTask;
using Worldex.Infrastructure.Interfaces;
using Worldex.Web.ApiModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using TwoFactorAuthNet;
using Worldex.Infrastructure.Data;
using Worldex.Core.ViewModels.FiatBankIntegration;

namespace Worldex.Web.Api
{
    [Route("api/[controller]")]
    public class ToDoItemsController : Controller
    {
        private readonly IRepository<ToDoItem> _todoRepository;
        private readonly IMediator _mediator;
        private readonly IWalletConfiguration _walletConfiguration;
        private FiatIntegrateRepository _fiatIntegrateRepository;
        private readonly ILogger<ToDoItemsController> _loggerFactory;
        //private readonly string sKey; //komal 03 May 2019, Cleanup
        private readonly IMessageConfiguration _messageConfiguration;
        private readonly ITrnMasterConfiguration _trnMasterConfiguration;
        private IPushNotificationsQueue<SendEmailRequest> _pushEmailQueue;
        private IPushNotificationsQueue<SendSMSRequest> _pushSMSQueue;
        private IPushNotificationsQueue<SendNotificationRequest> _pushNotificationQueue;
        private IMemoryCache _cache { get; set; }
        private RedisConnectionFactory _fact;
        public ToDoItemsController(RedisConnectionFactory Factory,ILogger<ToDoItemsController> loggerFactory, IRepository<ToDoItem> todoRepository, IMediator mediator, IMessageConfiguration messageConfiguration, ITrnMasterConfiguration trnMasterConfiguration, IPushNotificationsQueue<SendEmailRequest> PushEmailQueue, IPushNotificationsQueue<SendSMSRequest> PushSMSQueue, IPushNotificationsQueue<SendNotificationRequest> PushNotificationsQueue, IMemoryCache Cache, IWalletConfiguration walletConfiguration, FiatIntegrateRepository fiatIntegrateRepository)
        {
            _loggerFactory = loggerFactory/*.CreateLogger<ToDoItemsController>()*/;
            _todoRepository = todoRepository;
            _mediator = mediator;
            _messageConfiguration = messageConfiguration;
            _trnMasterConfiguration = trnMasterConfiguration;
            _pushEmailQueue = PushEmailQueue;
            _pushNotificationQueue = PushNotificationsQueue;
            _pushSMSQueue = PushSMSQueue;
            _cache = Cache;
            _fact = Factory;
            _walletConfiguration = walletConfiguration;
            _fiatIntegrateRepository = fiatIntegrateRepository;
        }

        // GET: api/ToDoItems
        [HttpGet]
        public IActionResult List()
        {
            // _loggerFactory logger = _loggerFactory.CreateLogger("LoggerCategory");
            // _loggerFactory.LogInformation("Your MSg");
            var logger = NLog.LogManager.GetCurrentClassLogger();
            // logger.Info("Hello World");
            logger.Info("Hello World");
            //  logger.Debug("Sample debug message");
            // logger.Log(LogLevel.Information, "Sample informational message");
            logger.Error("ow noos!", "");
            var items = _todoRepository.List()
                            .Select(item => ToDoItemDTO.FromToDoItem(item));
            return Ok(items);
        }

        // GET: api/ToDoItems
        [HttpGet("{id:int}")]
        [Authorize]
        public IActionResult GetById(int id)
        {
            var item = ToDoItemDTO.FromToDoItem(_todoRepository.GetById(id));
            return Ok(item);
        }

        // POST: api/ToDoItems
        [HttpPost]
        public IActionResult Post([FromBody] ToDoItemDTO item)
        {
            var todoItem = new ToDoItem()
            {
                Title = item.Title,
                Description = item.Description
            };
            _todoRepository.Add(todoItem);
            return Ok(ToDoItemDTO.FromToDoItem(todoItem));
        }

        [HttpPatch("{id:int}/complete")]
        public IActionResult Complete(int id)
        {
            var toDoItem = _todoRepository.GetById(id);
            toDoItem.MarkComplete();
            _todoRepository.Update(toDoItem);

            return Ok(ToDoItemDTO.FromToDoItem(toDoItem));
        }

        [Authorize]
        [HttpPost("SendMessage")]
        public async Task<IActionResult> SendMessage([FromBody] SendSMSRequest Request)
        {
            try
            {
                //CommunicationResponse Response = await _mediator.Send(Request);
                HelperForLog.WriteLogForSocket("ToDoItemsController", "0 SendMessage", " -Data- ");
                _pushSMSQueue.Enqueue(Request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("SendEmail")]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailRequest Request)
        {
            try
            {
                //CommunicationResponse Response = await _mediator.Send(Request);
                HelperForLog.WriteLogForSocket("ToDoItemsController", "0 SendEmail", " -Data- ");
                _pushEmailQueue.Enqueue(Request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost("SendNotification")]
        public async Task<IActionResult> SendNotification([FromBody] SendNotificationRequest Request)
        {
            try
            {

                //CommunicationResponse Response = await _mediator.Send(Request);
                _pushNotificationQueue.Enqueue(Request);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("EmailWithTemplate")]
        public async Task<IActionResult> EmailWithTemplateForSample([FromBody] SendEmailRequest Request)
        {
            try
            {
                string GeneratedLink = "https://cleandevtest.azurewebsites.net/SSO_Account/api/SignUp/ConfirmEmail?emailConfirmCode=mLjgF4N8iwzW2z4fs7dUSmEgAO1M3GziSngzVkS2UV9JAk1SUnCUoinNXm3SjGmcFlA6Tqp7wJTShohQ89Snbx3aastoWzItNncfTqf9dGqUNPPaCWAyumi%2B3FbcG1Jrh%2F8eRPznAZ%2BXKrwNDWz3JSniADZ4eDRE4e9mKbTk9rmc3OieMKv7nsco43TFOdkqsEqis%2Bxj5dKoPGx%2Bsk%2FWQnuhTl3j7u%2FtByvBIGT3c3EwgbyIlTdO6hR5ZIMG1JwZwRQ7Tl6UjlJBFc3AGMTAI7aRWN0LZNTjqcSJ6UzpKZhHt5%2FKhF8qdtP13S0HnNnt%2B2rpBVpve9Aw4t1R9Wez0aQn38axHNQhBwBSgqnDg0I%3D";
                IQueryable Result = await _messageConfiguration.GetTemplateConfigurationAsync(Convert.ToInt16(enCommunicationServiceType.Email), Convert.ToInt16(EnTemplateType.Registration), 0);
                foreach (TemplateMasterData Provider in Result)
                {
                    //string[] splitedarray = Provider.AdditionaInfo.Split(",");
                    //foreach (string s in splitedarray)
                    //{
                    Provider.Content = Provider.Content.Replace("#Link#", GeneratedLink);
                    Provider.Content = Provider.Content.Replace("###USERNAME###".ToUpper(), "Khushali");
                    Provider.Content = Provider.Content.Replace("###TYPE###".ToUpper(), "LTCBTC");
                    Provider.Content = Provider.Content.Replace("###REQAMOUNT###".ToUpper(), "100000");
                    Provider.Content = Provider.Content.Replace("###STATUS###".ToUpper(), "Success");
                    Provider.Content = Provider.Content.Replace("###USER###".ToUpper(), "Khushali");
                    Provider.Content = Provider.Content.Replace("###CURRENCY###".ToUpper(), "BTC");
                    Provider.Content = Provider.Content.Replace("###DATETIME###".ToUpper(), "4 Nov 2018 12:39 PM");
                    Provider.Content = Provider.Content.Replace("###AMOUNT###".ToUpper(), "100000");
                    Provider.Content = Provider.Content.Replace("###FEES###".ToUpper(), "1000");
                    Provider.Content = Provider.Content.Replace("###FINAL###".ToUpper(), "101000");
                    //}
                    Request.Body = Provider.Content;
                }

                CommunicationResponse Response = await _mediator.Send(Request);
                return Ok(Response);
            }
            catch (Exception ex)
            {
                return BadRequest(Response);
            }
        }

        [HttpGet("ReloadEmailMasterCache")]
        public async Task<IActionResult> ReloadEmailMasterCache()
        {
            try
            {

                IQueryable Result = await _messageConfiguration.GetAPIConfigurationAsync(Convert.ToInt32(enWebAPIRouteType.CommunicationAPI), Convert.ToInt32(enCommunicationServiceType.Email));
                var ConfigurationList = Result.Cast<CommunicationProviderList>().ToList().AsReadOnly();
                _cache.Set<IReadOnlyList<CommunicationProviderList>>("EmailConfiguration", ConfigurationList);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(Response);
            }
        }

        [HttpGet("ReloadtemplateMasterCache")]
        public async Task<IActionResult> ReloadtemplateMasterCache()
        {
            try
            {
                IList<TemplateMasterData> Result = _messageConfiguration.GetTemplateConfigurationAsyncV1();
                IReadOnlyList<TemplateMasterData> ConfigurationList = Result.ToList().AsReadOnly();
                _cache.Set<IReadOnlyList<TemplateMasterData>>("TemplateConfiguration", ConfigurationList);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(Response);
            }
        }

        [HttpGet("ReloadLTPdataCache")]
        public async Task<IActionResult> ReloadLTPdataCache()
        {
            try
            {
                IList<LPTPairFiat> Result = _fiatIntegrateRepository.GetPairForBinnance();
                IReadOnlyList<LPTPairFiat> ConfigurationList = Result.ToList().AsReadOnly();
                _cache.Set<IReadOnlyList<LPTPairFiat>>("LPTPairFiat", ConfigurationList);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(Response);
            }
        }

        [HttpGet("ReloadSMSMasterCache")]
        public async Task<IActionResult> ReloadSMSMasterCache()
        {
            try
            {

                IQueryable Result = await _messageConfiguration.GetAPIConfigurationAsync(Convert.ToInt32(enWebAPIRouteType.CommunicationAPI), Convert.ToInt32(enCommunicationServiceType.SMS));
                var ConfigurationList = Result.Cast<CommunicationProviderList>().ToList().AsReadOnly();
                _cache.Set<IReadOnlyList<CommunicationProviderList>>("SMSConfiguration", ConfigurationList);
                return Ok();

            }
            catch (Exception ex)
            {
                return BadRequest(Response);
            }
        }

        [HttpGet("ReloadCronMasterCache")]
        public async Task<IActionResult> ReloadCronMasterCache()
        {
            try
            {
                _walletConfiguration.GetCronMaster();
                _walletConfiguration.UpdateCronMasterList();
                return Ok(_walletConfiguration.GetCronMaster());
            }
            catch (Exception ex)
            {
                return BadRequest(Response);
            }
        }

        // POST api/values
        [HttpGet("GetQrCode")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetQrCode(string UserName)
        {
            try
            {
                TwoFactorAuth TFAuth = new TwoFactorAuth();
                string URL;
                string sKey = string.Empty;
                string sName = string.Empty;
                sKey = TFAuth.CreateSecret(160);
                sName = UserName; // dSetReq.Tables(0).Rows(0)("NAME");
                sKey = TFAuth.CreateSecret(160);
                URL = TFAuth.GetQrCodeImageAsDataUri(sName, sKey);
                string value = URL + "" + sKey;
                return Ok(new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = value, });

            }
            catch (Exception ex)
            {
                //return BadRequest(ex.ToString());               
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }

        }


        // POST api/values
        [HttpPost("VerifyQrCode")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> VerifyQrCode(string UserName, string key)
        {
            try
            {
                TwoFactorAuth TFAuth = new TwoFactorAuth();

                string sCode = UserName;
                string sKey = string.Empty;

                sKey = key; //TFAuth.CreateSecret(160);
                bool st = TFAuth.VerifyCode(sKey, sCode, 5);
                if (st)
                    return Ok(new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = "Success" });
                else
                    return Ok(new BizResponseClass { ReturnCode = enResponseCode.Success, ReturnMsg = "Fail" });

            }
            catch (Exception ex)
            {
                return BadRequest(new BizResponseClass { ReturnCode = enResponseCode.InternalError, ReturnMsg = ex.ToString(), ErrorCode = enErrorCode.Status500InternalServerError });
            }
        }

        [HttpGet("GetServiceSingleTon")]
        public ActionResult GetServiceSingleTon()
        {
            return Ok(_trnMasterConfiguration.GetServices());
        }

        [HttpGet("GetWTrnTypeMaster")]
        public ActionResult GetWTrnTypeMaster()
        {

            //_walletConfiguration.UpdateChargeTypeMasterList();
            return Ok(_walletConfiguration.GetCommissionTypeMaster());
        }

        //[HttpPost("Index1")]
        //public IActionResult Index1(ToDoItemDTO person)
        //{
        //    return DoSomething1(person);
        //}

        ////This action at /Person/IndexFromBody can bind JSON 
        //[HttpPost("IndexFromBody1")]
        //[Consumes("application/x-www-form-urlencoded", new[] { "application/json" })]
        //public IActionResult IndexFromBody1(ToDoItemDTO person)
        //{
        //    return DoSomething1(person);
        //}

        [HttpPost("TestingForsellersidewalletbalace")]
        public IActionResult DoSomething1(string Token,string WalletName)
        {
            // do something with the person here
            // ...
            var Redis = new RadisServices<ConnetedClientToken>(this._fact);
            IEnumerable<string> ClientList = Redis.GetKey(Token);
            foreach (string s in ClientList.ToList())
            {
                var Key = s;
                Key = Key.Split(":")[1].ToString();
                string Pair = Redis.GetPairOrMarketData(Key, ":", "Pairs");
                if (Pair.Split("_")[0].ToString() == WalletName)
                {
                    //_chatHubContext.Clients.Client(Key).SendAsync(Enum.GetName(typeof(enReturnMethod), enReturnMethod.RecieveSellerSideWalletBal), Data);
                    //Task.Run(() => HelperForLog.WriteLogForSocket("SellerSideWalletBal" + Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub", " Send Data :" + ));
                }
                else
                {
                    // ignore Data
                    //Task.Run(() => HelperForLog.WriteLogForSocket("SellerSideWalletBal" + Helpers.UTC_To_IST().ToString("dd/MM/yyyy HH:mm:ss.ffff"), "SocketHub", " Pair Subscription :" + Pair + " No data send :" + Data));
                }
            }
            return Ok();
        }


        [HttpPost("CheckBal")]
        public async Task<IActionResult> CheckBal([FromBody] LPBalanceCheck Request)
        {
            try
            {
                CancellationToken cancellationToken;
                var data = await _mediator.Send(Request, cancellationToken);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CheckBalArbitrage")]
        public async Task<IActionResult> CheckBalArbitrage([FromBody] LPBalanceCheckArbitrage Request)
        {
            try
            {
                CancellationToken cancellationToken;
                var data = await _mediator.Send(Request, cancellationToken);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CheckStatus")]
        public async Task<IActionResult> CheckStatus([FromBody] LPStatusCheckData Request)
        {
            try
            {
                CancellationToken cancellationToken;
                var data = await _mediator.Send(Request, cancellationToken);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("CheckStatusArbitrage")]
        public async Task<IActionResult> CheckStatusArbitrage([FromBody] LPStatusCheckDataArbitrage Request)
        {
            try
            {
                CancellationToken cancellationToken;
                var data = await _mediator.Send(Request, cancellationToken);
                return Ok(data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}