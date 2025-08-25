using Aptiverse.Application;
using Aptiverse.Infrastructure.Data;
using Aptiverse.Infrastructure.RateLimiting.Aptiverse.Infrastructure.RateLimiting;
using Aptiverse.Infrastructure.ServiceRegistartion;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Aptiverse.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
            services.AddAutoMapper(typeof(IAssemblyMarker).Assembly);
            services.AddApplicationServices();
            services.AddRateLimitingServices();
            services.AddLogging();

            return services;
        }
    }
}