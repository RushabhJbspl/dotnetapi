namespace MarketMaker.Infrastructure.Services.Redis
{
    public interface IRedisServices<T>
    {
        /// <summary>
        /// Get redis data stored in hash format
        /// </summary>
        /// <param name="key">redis data key</param>
        /// <returns> object of type T ViewModel.</returns>
        /// <remarks>-Sahil 28-09-2019</remarks>
        T Get(string key);
    }
}
