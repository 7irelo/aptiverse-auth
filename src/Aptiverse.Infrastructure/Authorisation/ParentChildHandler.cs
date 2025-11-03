using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Aptiverse.Infrastructure.Data;
using Aptiverse.Domain.Models.Students;

namespace Aptiverse.Infrastructure.Authorisation
{
    public class ParentChildHandler(ApplicationDbContext context) : AuthorizationHandler<ParentChildRequirement, Student>
    {
        private readonly ApplicationDbContext _context = context;

        protected override async Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            ParentChildRequirement requirement,
            Student student)
        {
            var parentUserId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(parentUserId))
            {
                return;
            }

            var isParent = await _context.StudentParents
                .Include(sp => sp.Parent)
                .AnyAsync(sp => sp.StudentId == student.Id && sp.Parent.UserId == parentUserId);

            if (isParent)
            {
                context.Succeed(requirement);
            }
        }
    }
}