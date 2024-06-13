namespace RateLimitLib.Abstraction
{
    public interface IRateLimitConfiguration
    {
        Task<bool> CheckRateLimit(string key);
    }

}