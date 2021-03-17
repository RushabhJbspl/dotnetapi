using MarketMaker.Application.Interfaces.Services;
using MarketMaker.Domain.Events;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LoggingNlog;
using MarketMaker.Application.ViewModels.Config;
using MarketMaker.Domain.Constants;
using Microsoft.Extensions.Options;

namespace MarketMaker.Application.DomainEventHandlers.FetchMarketMakerAuthToken
{
    public class MarketMakerAuthTokenChangedDomainEventHandler : INotificationHandler<MarketMakerAuthTokenChangedDomainEvent>
    {
        private readonly IWebUrlRequest _iWebUrlRequest;
        private readonly ICacheTokenService _iCacheTokenService;
        private readonly INLogger<MarketMakerAuthTokenChangedDomainEventHandler> _logger;
        private TokenApiConfigs _tokenApiConfigs;
        private MarketMakerConfigs _marketMakerConfigs;

        //statically stored credentials for marketmaker token generation -Sahil 04-10-2019 06:44 PM
        private readonly Dictionary<string, string> userAuthData;
        public MarketMakerAuthTokenChangedDomainEventHandler(IWebUrlRequest iWebUrlRequest, ICacheTokenService iCacheTokenService, IOptions<TokenApiConfigs> iTokenOptions, IOptions<MarketMakerConfigs> iMarketMakerOptions, INLogger<MarketMakerAuthTokenChangedDomainEventHandler> logger)
        {
            _iWebUrlRequest = iWebUrlRequest;
            _iCacheTokenService = iCacheTokenService;
            _logger = logger;
            _tokenApiConfigs = iTokenOptions.Value;
            _marketMakerConfigs = iMarketMakerOptions.Value;

            userAuthData = new Dictionary<string, string>
            {
                { "clientId", _marketMakerConfigs.clientId },
                { "grant_type", _marketMakerConfigs.grantType},
                {"username", _marketMakerConfigs.user},
                {"password", _marketMakerConfigs.passWord},
                {"scope", _marketMakerConfigs.scope }
            };

        }

        public Task Handle(MarketMakerAuthTokenChangedDomainEvent notification, CancellationToken cancellationToken)
        {
            _logger.WriteInfoLog("MarketMakerAuthTokenChangedDomainEventHandler", "request for token");
            _iCacheTokenService.RemoveStoreToken(Const.marketMakerTokenKey);

            string request = _iWebUrlRequest.GetFormUrlEncodedRequest(userAuthData);
            string resopnse = _iWebUrlRequest.Request(
               _tokenApiConfigs.tokenApiUrl,
               request,
               _tokenApiConfigs.tokenApiRequestMethod,
               _tokenApiConfigs.tokenApiContentType);

            if (resopnse != null) //possible WebException give null response -Sahil 09-10-2019 03:48 PM
            {
                var jObject = JObject.Parse(resopnse);

                _logger.WriteInfoLog("MarketMakerAuthTokenChangedDomainEventHandler", "request for token : success");
                _iCacheTokenService.SetToken(Const.marketMakerTokenKey, jObject["access_token"].ToString());
                return Task.CompletedTask;
            }

            #region commented code old event mechanism

            //remove beacuse change purpose of domain event -Sahil 09-10-2019 01:30 PM
            //if (_iMemoryCache.Get<string>(Const.marketMakerTokenKey) == null)
            //{
            //    string requestUrl = "http://localhost:60040/connect/token";
            //    string request = _iWebUrlRequest.GetFormUrlEncodedRequest(userAuthData);
            //    string resopnse = _iWebUrlRequest.Request(
            //       requestUrl,
            //       request,
            //       "POST",
            //       "application/x-www-form-urlencoded");

            //    var jObject = JObject.Parse(resopnse);
            //    _iMemoryCache.Set(Const.marketMakerTokenKey, jObject["access_token"].ToString());
            //}


            #endregion
            _logger.WriteInfoLog("MarketMakerAuthTokenChangedDomainEventHandler", "request for token : failed");
            return Task.CompletedTask;
        }
    }
}
