using System.Net;
using System.Threading.Tasks;
using EventBusRabbitMQ.EventHandlers;
using LoggingNlog;
using MarketMaker.Application.IntegrationEvents.Events;
using MarketMaker.Application.Interfaces.Queries;
using MarketMaker.Application.Interfaces.Services;
using MarketMaker.Application.ViewModels.Config;
using MarketMaker.Domain.Constants;
using MarketMaker.Domain.Enum;
using MarketMaker.Domain.Events;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace MarketMaker.Application.IntegrationEvents.EventHandling
{
    public class MarketMakerTransactionSettledIntegrationEventHandler : IIntegrationEventHandler<MarketMakerTransactionSettledIntegrationEvent>
    {
        private IWebUrlRequest _iWebUrlRequest;
        private ICacheTokenService _iCacheTokenService;
        private IMediator _iMediator;
        private INLogger<UserBalanceCheckCompletedIntegrationEventHandler> _logger;
        private readonly IMarketMakerQueries _iMarketMakerQueries;
        private OrderApiConfigs _orderApiConfigs;

        public MarketMakerTransactionSettledIntegrationEventHandler(IWebUrlRequest iWebUrlRequest, ICacheTokenService iCacheTokenService, IMediator iMediator, IOptions<OrderApiConfigs> iOptions, INLogger<UserBalanceCheckCompletedIntegrationEventHandler> logger, IMarketMakerQueries iMarketMakerQueries)
        {
            _iWebUrlRequest = iWebUrlRequest;
            _iCacheTokenService = iCacheTokenService;
            _iMediator = iMediator;
            _logger = logger;
            _iMarketMakerQueries = iMarketMakerQueries;
            _orderApiConfigs = iOptions.Value;
        }
        public Task Handle(MarketMakerTransactionSettledIntegrationEvent @event)
        {
            //get preference for OrderHoldOrderRateChange from database and manipulate price -Sahil 14-10-2019 12:42 PM
            decimal rateChange = _iMarketMakerQueries.GetMarketMakerHoldOrderRateChange(@event.currencyPairId).Result;

            if (@event.orderSide == (short)TransactionType.Buy)
                @event.price = @event.price - ((@event.price * rateChange) / 100);
            else if (@event.orderSide == (short)TransactionType.Sell)
                @event.price = @event.price + ((@event.price * rateChange) / 100);


            _logger.WriteInfoLog("UserBalanceCheckCompletedIntegrationEvent", $"UserBalanceCheckCompletedIntegrationEvent handler called for {@event.orderSide}, {{changed}} price: {@event.price}");
            string token = _iCacheTokenService.GetStoreToken(Const.marketMakerTokenKey);

            //if token has not cached it request for fetch -Sahil 09-10-2019 05:52 PM
            if (token == null)
            {
                _iMediator.Publish(new MarketMakerAuthTokenChangedDomainEvent());
                token = _iCacheTokenService.GetStoreToken(Const.marketMakerTokenKey);
            }

            //make buy/sell order api call -Sahil 09-10-2019 05:53 PM
            var response = _iWebUrlRequest.Request(
                _orderApiConfigs.orderApiUrl,
                JsonConvert.SerializeObject(@event),
                _orderApiConfigs.orderApiRequestMethod,
                _orderApiConfigs.orderApiContentType,
                new WebHeaderCollection
                {
                    {"Authorization",  $"Bearer {token}"}
                }
            );

            _logger.WriteInfoLog("Handle", $"order api call response: {response ?? "get null response"}");

            //response return null when get 401 due to token expired, retry for order -Sahil 09-10-2019 05:55 PM
            if (response == null)
            {
                _logger.WriteInfoLog("Handle", $"retry for {@event.orderSide}");

                _iMediator.Publish(new MarketMakerAuthTokenChangedDomainEvent()); //token expired make request -Sahil 09-10-2019 06:21 PM
                token = _iCacheTokenService.GetStoreToken(Const.marketMakerTokenKey);

                var retryResponse = _iWebUrlRequest.Request(
                    _orderApiConfigs.orderApiUrl,
                    JsonConvert.SerializeObject(@event),
                    _orderApiConfigs.orderApiRequestMethod,
                    _orderApiConfigs.orderApiContentType,
                    new WebHeaderCollection
                    {
                        {"Authorization",  $"Bearer {token}"}
                    }
                );
                _logger.WriteInfoLog("Handle", $"order api call response: {retryResponse ?? "get null response order not occured in system"}");

            }

            return Task.CompletedTask;
        }
    }
}
