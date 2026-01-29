using Aptiverse.Api.Domain.Repositories;
using Aptiverse.Api.Infrastructure.Caching;
using Aptiverse.Api.Infrastructure.Data;
using Aptiverse.Api.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Aptiverse.Api.Infrastructure
{
    public static class Registrations
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                       .EnableSensitiveDataLogging()
                       .EnableDetailedErrors());

            services.AddRedisServices(configuration);

            services.AddTransient<IRedisCacheService, RedisCacheService>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            return services;
        }

        private static IServiceCollection AddRedisServices(this IServiceCollection services, IConfiguration configuration)
        {
            var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379,abortConnect=false";

            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var config = ConfigurationOptions.Parse(redisConnectionString);
                config.AbortOnConnectFail = false;
                config.ConnectRetry = 3;
                config.ConnectTimeout = 5000;
                config.SyncTimeout = 5000;

                var logger = sp.GetRequiredService<ILogger<ConnectionMultiplexer>>();
                logger.LogInformation("Connecting to Redis: {RedisConnection}", redisConnectionString);

                return ConnectionMultiplexer.Connect(config);
            });

            services.AddTransient<IDatabase>(sp =>
            {
                var redis = sp.GetRequiredService<IConnectionMultiplexer>();
                return redis.GetDatabase();
            });

            services.AddHealthChecks()
                .AddRedis(redisConnectionString, name: "redis");

            return services;
        }
    }
}