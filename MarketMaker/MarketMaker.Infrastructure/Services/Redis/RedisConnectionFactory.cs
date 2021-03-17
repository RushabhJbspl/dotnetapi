using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;

namespace MarketMaker.Infrastructure.Services.Redis
{
    public class RedisConnectionFactory : IRedisConnectionFactory
    {
        private readonly IConfiguration _configuration;
        private readonly Lazy<ConnectionMultiplexer> _connection;

        public RedisConnectionFactory(IConfiguration configuration)
        {
            _configuration = configuration;

            ConfigurationOptions configurationOptions = new ConfigurationOptions()
            {
                AbortOnConnectFail = false,
                EndPoints = { configuration.GetValue<string>("RedisConfig:Host") }
            };
            _connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(configurationOptions));
        }
        public ConnectionMultiplexer Connection()
        {
            return _connection.Value;
        }
    }
}
