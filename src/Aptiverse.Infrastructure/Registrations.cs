using Aptiverse.Core.Services;
using Aptiverse.Domain.Interfaces;
using Aptiverse.Domain.Models;
using Aptiverse.Infrastructure.Data;
using Aptiverse.Infrastructure.Repositories;
using Aptiverse.Infrastructure.Services;
using Aptiverse.Infrastructure.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Aptiverse.Infrastructure
{
    public static class Registrations
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
                       .EnableSensitiveDataLogging()
                       .EnableDetailedErrors());

            services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;

                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.AddRedisServices(configuration);

            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ITokenProvider, TokenProvider>();
            services.AddScoped<ITokenStorageService, RedisTokenStorageService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddMemoryCache();

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
