using Aptiverse.Application;
using Aptiverse.Domain.Models.Users;
using Aptiverse.Infrastructure;
using Aptiverse.Infrastructure.Data;
using Aptiverse.Infrastructure.RateLimiting.Aptiverse.Infrastructure.RateLimiting;
using Aptiverse.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

namespace Aptiverse.Api.Web
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddInfrastructureServices(configuration);
            services.AddIdentityServices(configuration);
            services.AddRedisServices(configuration);
            services.AddAutoMapper(typeof(IAssemblyMarker).Assembly);
            services.AddRateLimitingServices();
            services.AddLogging();
            services.AddApplicationServices();

            return services;
        }

        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured"))),
                    ClockSkew = TimeSpan.Zero
                };

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogError(context.Exception, "JWT Authentication failed");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                        logger.LogInformation("JWT Token validated for user: {User}", context.Principal?.Identity?.Name);
                        return Task.CompletedTask;
                    }
                };
            })
            .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.Expiration = TimeSpan.FromMinutes(60);
                options.LoginPath = "/api/auth/external-login";
                options.LogoutPath = "/api/auth/logout";
                options.AccessDeniedPath = "/api/auth/access-denied";
            })
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"] ?? throw new InvalidOperationException("Google ClientId is not configured");
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"] ?? throw new InvalidOperationException("Google ClientSecret is not configured");
                options.SaveTokens = true;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.Always;

                options.Scope.Add("profile");
                options.Scope.Add("email");
            });
            services.AddAuthorizationBuilder()
                .AddPolicy("RequireAuthenticatedUser", policy =>
                    policy.RequireAuthenticatedUser())
                .AddPolicy("ApiScope", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("scope", "api");
                })
                .AddPolicy("Admin", policy =>
                    policy.RequireRole("Admin"))
                .AddPolicy("User", policy =>
                    policy.RequireRole("User", "Admin"));

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