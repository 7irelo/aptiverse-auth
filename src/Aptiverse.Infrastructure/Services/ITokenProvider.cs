using Aptiverse.Domain.Models.Users;
using System.Security.Claims;

namespace Aptiverse.Infrastructure.Services
{
    public interface ITokenProvider
    {
        Task<string> GenerateJwtTokenAsync(ApplicationUser user, IList<string> roles);
        Task<ClaimsPrincipal?> ValidateTokenAsync(string token);
        string GenerateRefreshToken();
        DateTime GetTokenExpiration(string token);
        string GetUserIdFromToken(string token);
        IEnumerable<Claim> GetClaimsFromToken(string token);
        bool IsTokenExpiringSoon(string token, int minutesThreshold = 5);
    }
}