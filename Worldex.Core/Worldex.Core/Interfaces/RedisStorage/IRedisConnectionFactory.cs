using StackExchange.Redis;

namespace Worldex.Core.Services.RadisDatabase
{
    public interface IRedisConnectionFactory
    {
        ConnectionMultiplexer Connection();
    }
}
