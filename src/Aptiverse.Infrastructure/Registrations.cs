using Aptiverse.Domain.Interfaces;
using Aptiverse.Domain.Models.Users;
using Aptiverse.Infrastructure.Data;
using Aptiverse.Infrastructure.Repositories;
using Aptiverse.Infrastructure.Services;
using Aptiverse.Infrastructure.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aptiverse.Infrastructure
{
    public static class Registrations
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName))
    .EnableSensitiveDataLogging()  // Add this line
    .EnableDetailedErrors());      // Add this line
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
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

            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<ITokenProvider, TokenProvider>();
            services.AddScoped<ITokenStorageService, RedisTokenStorageService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddMemoryCache();

            return services;
        }
    }
}