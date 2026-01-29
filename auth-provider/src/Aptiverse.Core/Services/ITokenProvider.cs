using Aptiverse.Domain.Models;
using System.Security.Claims;

namespace Aptiverse.Core.Services
{
    public interface ITokenProvider
    {
        Task<string> GenerateJwtTokenAsync(User user, IList<string> roles);
        Task<ClaimsPrincipal?> ValidateTokenAsync(string token);
        string GenerateRefreshToken();
        DateTime GetTokenExpiration(string token);
        string GetUserIdFromToken(string token);
        IEnumerable<Claim> GetClaimsFromToken(string token);
        bool IsTokenExpiringSoon(string token, int minutesThreshold = 5);
    }
}
