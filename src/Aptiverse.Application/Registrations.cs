using Aptiverse.Application.Admins.Services;
using Aptiverse.Application.Auth.Services;
using Aptiverse.Application.Parents.Services;
using Aptiverse.Application.Students.Services;
using Aptiverse.Application.Taechers.Services;
using Aptiverse.Application.Users.Services;
using Aptiverse.Core.Services.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Aptiverse.Application
{
    public static class Registrations
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IParentService, ParentService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
