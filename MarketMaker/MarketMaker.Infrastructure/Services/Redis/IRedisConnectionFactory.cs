using StackExchange.Redis;

namespace MarketMaker.Infrastructure.Services.Redis
{
    public interface IRedisConnectionFactory
    {
        ConnectionMultiplexer Connection();
    }
}
