namespace Aptiverse.Infrastructure.RateLimiting
{
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.RateLimiting;
    using Microsoft.Extensions.DependencyInjection;
    using System.Threading.RateLimiting;

    namespace Aptiverse.Infrastructure.RateLimiting
    {
        public static class RateLimitingConfiguration
        {
            public static IServiceCollection AddRateLimitingServices(this IServiceCollection services)
            {
                services.AddRateLimiter(options =>
                {
                    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                    {
                        var user = context.User;
                        var policyName = "default";

                        if (user.IsInRole("Admin") || user.IsInRole("SuperUser"))
                        {
                            policyName = "premium";
                        }
                        else if (user.Identity?.IsAuthenticated == true)
                        {
                            policyName = "authenticated";
                        }

                        return policyName switch
                        {
                            "premium" => RateLimitPartition.GetTokenBucketLimiter("premium", key =>
                                new TokenBucketRateLimiterOptions
                                {
                                    TokenLimit = 1000,
                                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                                    QueueLimit = 100,
                                    ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                                    TokensPerPeriod = 100,
                                    AutoReplenishment = true
                                }),
                            "authenticated" => RateLimitPartition.GetTokenBucketLimiter("authenticated", key =>
                                new TokenBucketRateLimiterOptions
                                {
                                    TokenLimit = 500,
                                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                                    QueueLimit = 50,
                                    ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                                    TokensPerPeriod = 50,
                                    AutoReplenishment = true
                                }),
                            _ => RateLimitPartition.GetFixedWindowLimiter("default", key =>
                                new FixedWindowRateLimiterOptions
                                {
                                    PermitLimit = 100,
                                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                                    QueueLimit = 10,
                                    Window = TimeSpan.FromMinutes(1),
                                    AutoReplenishment = true
                                })
                        };
                    });

                    options.OnRejected = async (context, token) =>
                    {
                        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                        await context.HttpContext.Response.WriteAsync("Too many requests. Please try again later.", token);
                    };
                });

                services.AddRateLimiter(limiterOptions =>
                {
                    limiterOptions.AddFixedWindowLimiter("FixedWindowPolicy", options =>
                    {
                        options.Window = TimeSpan.FromSeconds(10);
                        options.PermitLimit = 5;
                        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                        options.QueueLimit = 0;
                    });

                    limiterOptions.AddSlidingWindowLimiter("SlidingWindowPolicy", options =>
                    {
                        options.Window = TimeSpan.FromSeconds(15);
                        options.SegmentsPerWindow = 3;
                        options.PermitLimit = 20;
                        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                        options.QueueLimit = 5;
                    });

                    limiterOptions.AddTokenBucketLimiter("TokenBucketPolicy", options =>
                    {
                        options.TokenLimit = 100;
                        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                        options.QueueLimit = 10;
                        options.ReplenishmentPeriod = TimeSpan.FromSeconds(10);
                        options.TokensPerPeriod = 20;
                        options.AutoReplenishment = true;
                    });

                    limiterOptions.AddConcurrencyLimiter("ConcurrencyPolicy", options =>
                    {
                        options.PermitLimit = 10;
                        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                        options.QueueLimit = 5;
                    });
                });

                return services;
            }
        }
    }
}
