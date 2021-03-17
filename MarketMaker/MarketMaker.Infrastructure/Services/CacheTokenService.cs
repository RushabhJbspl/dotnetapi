using MarketMaker.Application.Interfaces.Services;
using MarketMaker.Domain.Events;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace MarketMaker.Infrastructure.Services
{
    public class CacheTokenService : ICacheTokenService
    {
        private readonly IWebUrlRequest _iWebUrlRequest;
        private readonly IMemoryCache _iMemoryCache;
        private readonly IMediator _iMediator;

        public CacheTokenService(IWebUrlRequest iWebUrlRequest, IMemoryCache iMemoryCache, IMediator iMediator)
        {
            _iWebUrlRequest = iWebUrlRequest;
            _iMemoryCache = iMemoryCache;
            _iMediator = iMediator;
        }

        public void SetToken(string tokenKey, string tokenValue)
        {
            _iMemoryCache.Set(tokenKey, tokenValue);
        }

        public string GetStoreToken(string tokenKey)
        {
            return _iMemoryCache.Get<string>(tokenKey);
        }

        public void RemoveStoreToken(string tokenKey)
        {
            _iMemoryCache.Remove(tokenKey);
        }
    }
}
