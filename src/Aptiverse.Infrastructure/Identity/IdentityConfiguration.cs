using Aptiverse.Domain.Models.Users;
using Aptiverse.Infrastructure.Authorisation;
using Aptiverse.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aptiverse.Infrastructure.Identity
{
    public static class IdentityConfiguration
    {
        public static IServiceCollection AddIdentityServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
            });

            // Add JWT authentication
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
                    )
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireStudent", p => p.RequireRole("Student"));
                options.AddPolicy("RequireTeacher", p => p.RequireRole("Teacher"));
                options.AddPolicy("RequireAdmin", p => p.RequireRole("Admin"));
                options.AddPolicy("RequireSuperUser", p => p.RequireRole("SuperUser"));
                options.AddPolicy("ParentAccess", policy => policy.Requirements.Add(new ParentChildRequirement()));
            });

            services.AddScoped<IAuthorizationHandler, ParentChildHandler>();

            return services;
        }
    }
}
