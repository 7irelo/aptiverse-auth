using Aptiverse.Application.Admins.Services;
using Aptiverse.Application.AI.Services;
using Aptiverse.Application.Parents.Services;
using Aptiverse.Application.Students.Services;
using Aptiverse.Application.Teachers.Services;
using Aptiverse.Application.Users.Services;
using Aptiverse.Domain.Interfaces;
using Aptiverse.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Aptiverse.Infrastructure.ServiceRegistartion
{
    public static class ServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddInfrastructureServices()
                    .AddUserManagementServices()
                    .AddModelServices();

            return services;
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            return services;
        }

        public static IServiceCollection AddUserManagementServices(this IServiceCollection services)
        {
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IParentService, ParentService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<IUserService, UserService>();
            return services;
        }

        public static IServiceCollection AddModelServices(this IServiceCollection services)
        {
            services.AddScoped<IModelTaskService, ModelTaskService>();
            return services;
        }
    }
}