using MarketMaker.Application.IntegrationEvents.Events;
using MarketMaker.Domain.Events;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using LoggingNlog;
using System.Threading;

namespace MarketMaker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly IMediator imediator;
        private readonly INLogger<ValuesController> _logger;

        //done test resource commented -Sahil 07-10-2019 12:07 PM
        //private readonly IMarketMakerQueries _iMarketMakerQueries;
        //private readonly IRedisTradingManagement _redisTradingManagement;
        //private readonly IWebUrlRequest _webUrlRequest;

        //public ValuesController(IMarketMakerQueries iMarketMakerQueries, IRedisTradingManagement redisTradingManagement, IWebUrlRequest webUrlRequest)
        //{
        //    _iMarketMakerQueries = iMarketMakerQueries;
        //    _redisTradingManagement = redisTradingManagement;
        //    _webUrlRequest = webUrlRequest;
        //}

        // comment test resource -Sahil 07-10-2019
        public ValuesController(IMediator imediator, INLogger<ValuesController> logger)
        {
            this.imediator = imediator;
            _logger = logger;
        }

        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            //Testing
            //TransactionOnHoldCompletedIntegrationEvent @event = new TransactionOnHoldCompletedIntegrationEvent(35, 1, TransactionType.Sell, 7230, 1, "BTC_USDT");
            //new TransactionOnHoldCompletedIntegrationEventHandler(_iMarketMakerQueries, _redisTradingManagement).Handle(@event);
            //var dict = new Dictionary<string, string>();
            //dict.Add("clientId", "cleanarchitecture");
            //dict.Add("grant_type", "password");
            //dict.Add("username", "Nishant");
            //dict.Add("password", "Admin@123$");
            //dict.Add("scope", "openid profile email offline_access client_id roles phone");

            ////testing get marketmaker url token for purform buy/sell transaction -Sahil 04-10-2019 05:47 PM
            ////string data = $"clientId=cleanarchitecture&grant_type=password&username=Nishant&password=Admin@123$&scope={HttpUtility.UrlEncode("openid profile email offline_access client_id roles phone")}";
            //string data = _webUrlRequest.GetFormUrlEncodedRequest(dict);
            //_webUrlRequest.Request(
            //    url: "http://localhost:60040/connect/token",
            //    request: data,
            //    methodType: "POST",
            //    contentType: "application/x-www-form-urlencoded"
            //    );

            //test FetchAuthTokenDomainEvent testing -Sahil 07-10-2019 12:05 PM
            //imediator.Publish(new MarketMakerAuthTokenChangedDomainEvent());
            //_logger.WriteInfoLog("ActionResult", "log test");
            //test buy/ sell api call -Sahil 07 - 10 - 2019 04:41 PM
            //imediator.Publish(new UserBalanceCheckCompletedIntegrationEvent(
            //    10031001,
            //    7230,
            //   2,
            //    4));

            //test mediator handler call twice -Sahil 09-10-2019 06:41 PM
            //imediator.Publish(new TestDomainEvent());

            return new string[] { "Running...value controller..."};
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
