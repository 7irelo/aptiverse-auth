using Aptiverse.Domain.Models.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Aptiverse.Infrastructure.Services
{
    public class TokenProvider : ITokenProvider
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _key;
        private readonly ITokenStorageService _tokenStorageService;
        private readonly ILogger<TokenProvider> _logger;

        public TokenProvider(
            IConfiguration configuration,
            ITokenStorageService tokenStorageService,
            ILogger<TokenProvider> logger)
        {
            _configuration = configuration;
            _tokenStorageService = tokenStorageService;
            _logger = logger;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        }

        public async Task<string> GenerateJwtTokenAsync(ApplicationUser user, IList<string> roles)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("firstName", user.FirstName ?? ""),
                new Claim("lastName", user.LastName ?? "")
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);
            var expiry = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpireHours"] ?? "4"));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiry,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            try
            {
                await _tokenStorageService.StoreTokenAsync(user.Id, tokenString, expiry);
                _logger.LogDebug("JWT token stored for user {UserId}", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to store token for user {UserId}", user.Id);
            }

            return tokenString;
        }

        public async Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogWarning("Token validation failed: Token is null or empty");
                return null;
            }

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    IssuerSigningKey = _key,
                    ClockSkew = TimeSpan.Zero,
                    // Add this to ensure proper claim mapping
                    NameClaimType = ClaimTypes.NameIdentifier
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                // Look for user ID using multiple possible claim types
                var userId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                            principal.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    _logger.LogWarning("Token validation failed: User ID not found in token. Available claims: {Claims}",
                        string.Join(", ", principal.Claims.Select(c => $"{c.Type}:{c.Value}")));
                    throw new SecurityTokenException("Invalid token claims - missing user ID");
                }

                _logger.LogDebug("Token validation successful for user {UserId}", userId);

                // Check Redis token storage
                var isValid = await _tokenStorageService.IsTokenValidAsync(userId, token);
                if (!isValid)
                {
                    _logger.LogWarning("Token validation failed: Token revoked or not found in Redis for user {UserId}", userId);
                    throw new SecurityTokenException("Token has been revoked");
                }

                _logger.LogInformation("Token fully validated for user {UserId}", userId);
                return principal;
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogWarning("Token validation failed: Token expired - {Message}", ex.Message);
                throw;
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning("Token validation failed: Security token error - {Message}", ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Token validation failed: Unexpected error");
                return null;
            }
        }

        public string GenerateRefreshToken()
        {
            try
            {
                var randomNumber = new byte[32];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to generate refresh token");
                throw;
            }
        }

        public DateTime GetTokenExpiration(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be null or empty", nameof(token));

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                // Read token without validation (we just want expiration)
                var jwtToken = tokenHandler.ReadJwtToken(token);

                if (jwtToken.ValidTo == DateTime.MinValue)
                    throw new SecurityTokenException("Token does not have expiration claim");

                return jwtToken.ValidTo;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get token expiration");
                throw new SecurityTokenException("Invalid token format", ex);
            }
        }

        public string GetUserIdFromToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be null or empty", nameof(token));

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

                if (string.IsNullOrEmpty(userId))
                    throw new SecurityTokenException("User ID not found in token");

                return userId;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get user ID from token");
                throw new SecurityTokenException("Invalid token format", ex);
            }
        }

        // Additional helper method to get all claims from token without validation
        public IEnumerable<Claim> GetClaimsFromToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be null or empty", nameof(token));

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);
                return jwtToken.Claims;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get claims from token");
                throw new SecurityTokenException("Invalid token format", ex);
            }
        }

        // Helper method to check if token is about to expire (for refresh scenarios)
        public bool IsTokenExpiringSoon(string token, int minutesThreshold = 5)
        {
            try
            {
                var expiration = GetTokenExpiration(token);
                return expiration <= DateTime.UtcNow.AddMinutes(minutesThreshold);
            }
            catch
            {
                return true; // If we can't read expiration, treat as expired
            }
        }
    }
}