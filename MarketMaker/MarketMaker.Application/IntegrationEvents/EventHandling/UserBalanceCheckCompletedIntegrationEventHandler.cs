using MarketMaker.Application.IntegrationEvents.Events;
using MarketMaker.Application.Interfaces.Services;
using MarketMaker.Domain.Constants;
using MediatR;
using Newtonsoft.Json;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using LoggingNlog;
using MarketMaker.Application.ViewModels.Config;
using MarketMaker.Domain.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

namespace MarketMaker.Application.IntegrationEvents.EventHandling
{
    //TODO subscribe handler with rabbitMQ -Sahil 07-10-2019 01:22 PM
    //for testing handle event with mediatr -Sahil 07-10-2019 03:07 PM
    public class UserBalanceCheckCompletedIntegrationEventHandler : INotificationHandler<UserBalanceCheckCompletedIntegrationEvent>
    {
        private readonly IWebUrlRequest _iWebUrlRequest;
        private readonly ICacheTokenService _iCacheTokenService;
        private readonly IMediator _iMediator;
        private readonly INLogger<UserBalanceCheckCompletedIntegrationEventHandler> _logger;
        private OrderApiConfigs _orderApiConfigs;

        public UserBalanceCheckCompletedIntegrationEventHandler(IWebUrlRequest iWebUrlRequest, ICacheTokenService iCacheTokenService, IMediator iMediator, IOptions<OrderApiConfigs> iOptions, INLogger<UserBalanceCheckCompletedIntegrationEventHandler> logger)
        {
            _iWebUrlRequest = iWebUrlRequest;
            _iCacheTokenService = iCacheTokenService;
            _iMediator = iMediator;
            _logger = logger;
            _orderApiConfigs = iOptions.Value;
        }

        public Task Handle(UserBalanceCheckCompletedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            _logger.WriteInfoLog("UserBalanceCheckCompletedIntegrationEvent", $"UserBalanceCheckCompletedIntegrationEvent handler called for {notification.orderSide}, price: {notification.price}");
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
                JsonConvert.SerializeObject(notification),
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
                _logger.WriteInfoLog("Handle", $"retry for {notification.orderSide}");

                _iMediator.Publish(new MarketMakerAuthTokenChangedDomainEvent()); //token expired make request -Sahil 09-10-2019 06:21 PM
                token = _iCacheTokenService.GetStoreToken(Const.marketMakerTokenKey);

                var retryResponse = _iWebUrlRequest.Request(
                     _orderApiConfigs.orderApiUrl,
                     JsonConvert.SerializeObject(notification),
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
