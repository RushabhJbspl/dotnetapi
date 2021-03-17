namespace Worldex.Core.Services.RadisDatabase
{
    public class RedisConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string Name { get; set; }
        public int TimeoutMilliseconds { get; set; } = 15000;
    }
}
