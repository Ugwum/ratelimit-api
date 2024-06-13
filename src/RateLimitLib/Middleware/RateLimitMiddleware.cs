using Microsoft.AspNetCore.Http;
using RateLimitLib.Abstraction;

namespace RateLimitLib.Middleware
{
    public class RateLimitMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly IRateLimitConfiguration _rateLimitConfiguration;

        public RateLimitMiddleware(RequestDelegate next, IRateLimitConfiguration rateLimitConfiguration)
        {
            _next = next;
            _rateLimitConfiguration = rateLimitConfiguration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var rateLimitKey = context.Connection.RemoteIpAddress.ToString();
            var isRateLimited = await _rateLimitConfiguration.CheckRateLimit(rateLimitKey);

            if (!isRateLimited)
            {
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                await context.Response.WriteAsync("Rate limit exceeded.");
                return;
            }

            await _next(context);
        }
    }
}
