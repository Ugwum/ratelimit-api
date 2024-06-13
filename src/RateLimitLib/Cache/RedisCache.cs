using Newtonsoft.Json;
using RateLimitLib.Abstraction;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RateLimitLib.Cache
{
    public class RedisCache : ICacheProvider
    {

        private readonly IConnectionMultiplexer _connectionMutiplexer;
        public RedisCache(IConnectionMultiplexer connectionMultiplexer)
        {
            _connectionMutiplexer = connectionMultiplexer;
        }
        public IDatabase GetDatabase()
        {
            return _connectionMutiplexer.GetDatabase();
        }
        public async Task<T> GetAsync<T>(string cacheKey)
        {
            var db = _connectionMutiplexer.GetDatabase();

            var resultString = await db.StringGetAsync(cacheKey);

            T result = default;

            if (!resultString.IsNullOrEmpty)
            {
                result = JsonConvert.DeserializeObject<T>(resultString);
            }

            return result;
        }

        public async Task SetAsync<T>(string key, T value, double expiryMinutes)
        {
            var db = _connectionMutiplexer.GetDatabase();
            var serializedData = JsonConvert.SerializeObject(value);
            await db.StringSetAsync(key, serializedData, TimeSpan.FromMinutes(expiryMinutes));
            await db.KeyExpireAsync(key, TimeSpan.FromMinutes(expiryMinutes));
        }

        public async Task SetAsync<T>(string key, T value)
        {
            var db = _connectionMutiplexer.GetDatabase();
            var serializedData = JsonConvert.SerializeObject(value);
            await db.StringSetAsync(key, serializedData);
        }

        public void Set<T>(string key, T value)
        {
            var db = _connectionMutiplexer.GetDatabase();
            var serializedData = JsonConvert.SerializeObject(value);
            db.StringSet(key, serializedData);
        }

        public void Set<T>(string key, T value, double expiry)
        {
            var db = _connectionMutiplexer.GetDatabase();
            var serializedData = JsonConvert.SerializeObject(value);
            db.StringSet(key, serializedData, TimeSpan.FromMinutes(expiry));
            db.KeyExpire(key, TimeSpan.FromMinutes(expiry));
        }
        public T Get<T>(string cacheKey)
        {

            var db = _connectionMutiplexer.GetDatabase();

            var resultString = db.StringGet(cacheKey);

            T result = default;

            if (!resultString.IsNullOrEmpty)
            {
                result = JsonConvert.DeserializeObject<T>(resultString);
            }
            return result;
        }
        public async Task RemoveAsync<T>(string key)
        {

            var db = _connectionMutiplexer.GetDatabase();
            await db.KeyDeleteAsync(key);

        }

        public void Remove<T>(string key)
        {

            var db = _connectionMutiplexer.GetDatabase();
            db.KeyDelete(key);

        }       
    }
}
