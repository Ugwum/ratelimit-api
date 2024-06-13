using RateLimitLib.Abstraction;
using RateLimitLib.Cache;
using RateLimitLib.Configuration;
using RateLimitLib.Middleware;
using StackExchange.Redis;

namespace RateLimitDemoApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            var redisConnection = builder.Configuration.GetSection("RedisConnection").Get<RedisConnection>();
             
            builder.Services.AddSingleton<IConnectionMultiplexer>(x => ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                Ssl = redisConnection.UseSSL,
                ConnectRetry = 5,
                ConnectTimeout = 5000,
                SyncTimeout = 5000,
                DefaultDatabase = 0,
                EndPoints = { { redisConnection.Host, Convert.ToInt32(redisConnection.Port) } },
                Password = redisConnection.Password,
            }));

            builder.Services.AddSingleton<ICacheProvider, RedisCache>();

            builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            var limitRules = builder.Configuration.GetSection("RateLimit").Get<RateLimitOptions>();
            builder.Services.Configure<RateLimitOptions>(builder.Configuration.GetSection("RateLimit"));
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<RateLimitMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}