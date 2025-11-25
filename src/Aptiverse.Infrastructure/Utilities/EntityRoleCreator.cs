using Aptiverse.Domain.Models;
using Aptiverse.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace Aptiverse.Infrastructure.Utilities
{
    public static class EntityRoleCreator
    {
        public async static Task<IdentityResult> CreateRoleSpecificEntity(ApplicationDbContext dbContext, string userId, string userType)
        {
            try
            {
                switch (userType.ToLower())
                {
                    case "student":
                        dbContext.Students.Add(new Student { UserId = userId });
                        break;
                    case "teacher":
                        dbContext.Teachers.Add(new Teacher { UserId = userId });
                        break;
                    case "parent":
                        dbContext.Parents.Add(new Parent { UserId = userId });
                        break;
                    case "admin":
                        dbContext.Admins.Add(new Admin { UserId = userId });
                        break;
                    case "superuser":
                        dbContext.Superusers.Add(new Superuser { UserId = userId });
                        break;
                    default:
                        return IdentityResult.Failed(new IdentityError
                        {
                            Description = $"Invalid user type: {userType}. Must be Student, Teacher, Parent, Admin, or SuperUser."
                        });
                }

                await dbContext.SaveChangesAsync();
                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Description = $"Failed to create {userType} entity: {ex.Message}"
                });
            }
        }
    }
}
