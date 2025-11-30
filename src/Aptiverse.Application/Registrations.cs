using Aptiverse.Application.Auth.Services;
using Aptiverse.Application.Users.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aptiverse.Application
{
    public static class Registrations
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
