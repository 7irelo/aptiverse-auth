namespace Aptiverse.Api.Infrastructure.Caching
{
    public class CacheSettings
    {
        public int DefaultExpirationMinutes { get; set; } = 30;
        public bool Enabled { get; set; } = true;
        public string RedisConnectionString { get; set; } = "localhost:6379";
    }
}