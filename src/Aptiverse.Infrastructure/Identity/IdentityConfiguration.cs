using Aptiverse.Domain.Models.Users;
using Aptiverse.Infrastructure.Authorisation;
using Aptiverse.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
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
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
                    ),
                    ClockSkew = TimeSpan.Zero
                };
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireStudent", p => p.RequireRole("Student"));
                options.AddPolicy("RequireTeacher", p => p.RequireRole("Teacher"));
                options.AddPolicy("RequireParent", p => p.RequireRole("Parent"));
                options.AddPolicy("RequireAdmin", p => p.RequireRole("Admin"));
                options.AddPolicy("RequireSuperUser", p => p.RequireRole("SuperUser"));
                options.AddPolicy("ParentAccess", policy => policy.Requirements.Add(new ParentChildRequirement()));
                options.AddPolicy("RequireStaff", p => p.RequireRole("Teacher", "Admin", "SuperUser"));
                options.AddPolicy("RequireAnyRole", p => p.RequireRole("Student", "Teacher", "Parent", "Admin", "SuperUser"));
            });

            services.AddScoped<IAuthorizationHandler, ParentChildHandler>();

            return services;
        }
    }
}