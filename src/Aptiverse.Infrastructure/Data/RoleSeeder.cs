using Microsoft.AspNetCore.Identity;

namespace Aptiverse.Infrastructure.Data
{
    public static class RoleSeeder
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles =
            {
                "Superuser",
                "Admin",
                "Teacher",
                "Parent",
                "Student",
                "Tutor"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole
                    {
                        Name = role,
                        NormalizedName = role.ToUpper()
                    });
                }
            }
        }
    }
}
