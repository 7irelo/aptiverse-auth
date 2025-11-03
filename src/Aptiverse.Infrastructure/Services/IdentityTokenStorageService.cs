using Aptiverse.Domain.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Aptiverse.Infrastructure.Services
{
    public class IdentityTokenStorageService(
        UserManager<ApplicationUser> userManager,
        ILogger<IdentityTokenStorageService> logger) : ITokenStorageService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ILogger<IdentityTokenStorageService> _logger = logger;

        public async Task StoreTokenAsync(string userId, string token, DateTime expiry)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError("User with ID {UserId} not found when storing token", userId);
                throw new InvalidOperationException($"User with ID {userId} not found");
            }

            var tokenHash = HashToken(token);
            var tokenName = $"AT_{tokenHash}";

            _logger.LogInformation("Storing token reference for user {UserId}. Token hash: {TokenHash}", userId, tokenHash);

            var tokenValue = expiry.ToString("O");

            var result = await _userManager.SetAuthenticationTokenAsync(
                user,
                "JWT",
                tokenName,
                tokenValue);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogError("Failed to store token for user {UserId}: {Errors}", userId, errors);
                throw new InvalidOperationException($"Failed to store token: {errors}");
            }

            _logger.LogInformation("Token reference stored for user {UserId} until {Expiry}", userId, expiry);
        }

        public async Task<bool> IsTokenValidAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found when validating token", userId);
                return false;
            }

            var tokenHash = HashToken(token);
            var tokenName = $"AT_{tokenHash}";

            _logger.LogDebug("Checking token validity for user {UserId}. Token hash: {TokenHash}", userId, tokenHash);

            var storedExpiry = await _userManager.GetAuthenticationTokenAsync(user, "JWT", tokenName);

            if (string.IsNullOrEmpty(storedExpiry))
            {
                _logger.LogDebug("No stored token found for user {UserId} with hash {TokenHash}", userId, tokenHash);
                return false;
            }

            if (DateTime.TryParse(storedExpiry, out DateTime expiry) && expiry > DateTime.UtcNow)
            {
                _logger.LogDebug("Token valid for user {UserId}", userId);
                return true;
            }

            _logger.LogInformation("Token expired for user {UserId}, removing from storage", userId);
            await RevokeTokenAsync(userId, token);
            return false;
        }

        public async Task RevokeTokenAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found when revoking token", userId);
                return;
            }

            var tokenHash = HashToken(token);
            var tokenName = $"AT_{tokenHash}";

            await _userManager.RemoveAuthenticationTokenAsync(user, "JWT", tokenName);
            _logger.LogInformation("Token revoked for user {UserId}", userId);
        }

        public async Task RevokeAllUserTokensAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found when revoking all tokens", userId);
                return;
            }

            await _userManager.UpdateSecurityStampAsync(user);
            _logger.LogInformation("All tokens invalidated for user {UserId} via security stamp update", userId);
        }

        private static string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashBytes)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
        }

        public async Task CleanExpiredTokensAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return;

            _logger.LogDebug("Manual token cleanup not implemented for user {UserId}", userId);
        }
    }
}