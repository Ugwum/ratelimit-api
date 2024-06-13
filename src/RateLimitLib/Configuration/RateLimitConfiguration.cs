using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RateLimitLib.Abstraction;

namespace RateLimitLib.Configuration
{
    public class RateLimitConfiguration : IRateLimitConfiguration
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IOptions<RateLimitOptions> _rateLimitOptions;
        private readonly ICacheProvider _cacheProvider;
        private readonly ILogger<RateLimitConfiguration> _logger;
        public RateLimitConfiguration(IHttpContextAccessor httpContextAccessor,
            IOptions<RateLimitOptions> rateLimitOptions, ICacheProvider cacheProvider, ILogger<RateLimitConfiguration> logger)
        {
            _contextAccessor = httpContextAccessor;
            _rateLimitOptions = rateLimitOptions;
            _cacheProvider = cacheProvider;
            _logger = logger;
        }

        /// <summary>
        /// Check the ratelimit for a particular url based on the configured ratelimit for each endpoints 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> CheckRateLimit(string key)
        {
            var request = _contextAccessor.HttpContext.Request;
            var endpoint = request.Path.ToString().ToLowerInvariant();
            var ip_EndpointKey = $"{key}:{endpoint}";

            _logger.LogInformation($"Endpointkey : {ip_EndpointKey}");
            var rule = _rateLimitOptions.Value.RateLimitRules.FirstOrDefault(r => r.Endpoint == endpoint);

            if (rule == null)
            {
                return true;
            }
            var time = Convert.ToDouble(rule.Period.TotalMinutes);

            var iprateLimitEntry = await _cacheProvider.GetAsync<string>(ip_EndpointKey);
            _logger.LogInformation($" IPratelimitentry is  {iprateLimitEntry}");

            if (iprateLimitEntry == null)
            {
                _logger.LogInformation($" IPratelimitentry is null");
                await _cacheProvider.SetAsync(ip_EndpointKey, "1", time);
                iprateLimitEntry = "1";
            }

            var ipTryCount = int.Parse(iprateLimitEntry);

            if (ipTryCount >= rule.Limit) { return false; }

            await _cacheProvider.SetAsync(ip_EndpointKey, (ipTryCount + 1).ToString(), Convert.ToDouble(time));

            return true;

        }
    }
}
