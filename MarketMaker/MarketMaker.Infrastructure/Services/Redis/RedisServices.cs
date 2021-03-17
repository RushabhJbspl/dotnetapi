using StackExchange.Redis;

namespace MarketMaker.Infrastructure.Services.Redis
{
    /// <summary>
    /// class code have reference:
    /// <seealso cref="CleanArchitecture.Core.IRedisServices"/> 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RedisServices<T> : BaseService<T>, IRedisServices<T> where T : class
    {
        private readonly IRedisConnectionFactory _iConnectionFactory;
        private readonly IDatabase _iDatabase;

        public RedisServices(IRedisConnectionFactory iConnectionFactory)
        {
            _iConnectionFactory = iConnectionFactory;
            _iDatabase = _iConnectionFactory.Connection().GetDatabase();
        }

        public T Get(string key)
        {
            key = this.GenerateKey(key);
            var hash = _iDatabase.HashGetAll(key);
            return this.MapFromHash(hash);
        }
    }
}
