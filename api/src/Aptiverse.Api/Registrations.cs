using Aptiverse.Api.Application;
using Aptiverse.Api.Infrastructure;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Aptiverse.Api
{
    public static class Registrations
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddApplicationServices();
            services.AddInfrastructureServices(configuration);
            services.AddIdentityServices(configuration);
            services.AddAutoMapper(configuration =>
            {
                configuration.AllowNullCollections = true;
                configuration.AllowNullDestinationValues = true;
            }, typeof(IApplicationAssemblyMarker).Assembly);
            services.AddCorsServices(configuration);
            services.AddLogging();
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
            services.AddControllers(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
                //options.Filters.Add<NullResultFilter>();
                //options.Filters.Add<ValidationFilter>();
                //options.Filters.Add<ExceptionHandlingFilter>();
                //options.Filters.Add<LoggingFilter>();
            });

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
    }
}