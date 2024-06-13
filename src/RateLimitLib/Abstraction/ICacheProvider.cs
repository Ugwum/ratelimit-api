using StackExchange.Redis;

namespace RateLimitLib.Abstraction
{
    public interface ICacheProvider
    {

        Task<T> GetAsync<T>(string key);

        IDatabase GetDatabase();

        Task SetAsync<T>(string key, T value, double expiry);

        Task SetAsync<T>(string key, T value);

        void Set<T>(string key, T value);

        void Set<T>(string key, T value, double expiry);

        T Get<T>(string cacheKey);
        Task RemoveAsync<T>(string key);

        void Remove<T>(string key);
    }

}