using Aptiverse.Application;
using Aptiverse.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

namespace Aptiverse.Auth
{
    public static class Registrations
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddInfrastructureServices(configuration);
            services.AddCorsServices(configuration);
            services.AddIdentityServices(configuration);
            services.AddAutoMapper(configuration =>
            {
                configuration.AllowNullCollections = true;
                configuration.AllowNullDestinationValues = true;
            }, typeof(IApplicationAssemblyMarker).Assembly);
            services.AddRateLimitingServices();
            services.AddLogging();
            services.AddApplicationServices();

            return services;
        }

        public static IServiceCollection AddCorsServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowNextJS", policy =>
                {
                    policy.WithOrigins(
                            "http://localhost:3000",
                            "https://localhost:3000",
                            "http://127.0.0.1:3000",
                            "https://aptiverse.co.za",
                            "https://www.aptiverse.co.za"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });

                if (configuration.GetValue<bool>("EnableDevelopmentCors"))
                {
                    options.AddPolicy("AllowAll", policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                    });
                }
            });

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
