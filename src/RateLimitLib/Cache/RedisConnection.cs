namespace RateLimitLib.Cache
{
    public class RedisConnection
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string Password { get; set; }
        public bool UseSSL { get; set; }

    }
}
