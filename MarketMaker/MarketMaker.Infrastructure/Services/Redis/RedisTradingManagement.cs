using MarketMaker.Application.Interfaces.Services.Redis;
using MarketMaker.Application.ViewModels.Services.Redis;
using System;
using System.Threading.Tasks;
using LoggingNlog;

namespace MarketMaker.Infrastructure.Services.Redis
{
    public class RedisTradingManagement : IRedisTradingManagement
    {
        private readonly IRedisConnectionFactory _iConnectionFactory;
        private readonly INLogger<RedisTradingManagement> _logger;

        public RedisTradingManagement(IRedisConnectionFactory iConnectionFactory, INLogger<RedisTradingManagement> logger)
        {
            _iConnectionFactory = iConnectionFactory;
            _logger = logger;
        }
        public async Task<TickerDataViewModel> GetTickerDataAsync(string lpName, string pairName)
        {
            TickerDataViewModel data = null;
            try
            {
                IRedisServices<TickerDataViewModel> redisServices = new RedisServices<TickerDataViewModel>(_iConnectionFactory);
                string redisTickerDataKey = GetOrderTickerDataPathKey(lpName, pairName);

                data = redisServices.Get(redisTickerDataKey) ?? throw new Exception("Ticker Data retrieve from redis failed");
                return await Task.FromResult(data);
            }
            catch (Exception e)
            {
                _logger.WriteInfoLog("GetTickerDataAsync", $"exception occur while retrieving {lpName}:{pairName} data, check error log");
                _logger.WriteErrorLog("GetTickerDataAsync", e);
            }
            return await Task.FromResult(data);
        }

        private string GetOrderTickerDataPathKey(string lpName, string pairName)
        {
            return $"{lpName}:{pairName}";
        }
    }
}
