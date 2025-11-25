using Aptiverse.Core.Services;
using Aptiverse.Domain.Models;
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
        private readonly ILogger<TokenProvider> _logger;
        private readonly ITokenStorageService _tokenStorageService;


        public TokenProvider(
            IConfiguration configuration,
            ILogger<TokenProvider> logger,
            ITokenStorageService tokenStorageService)
        {
            _configuration = configuration;
            _logger = logger;
            _tokenStorageService = tokenStorageService;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        }

        public async Task<string> GenerateJwtTokenAsync(User user, IList<string> roles)
        {
            ArgumentNullException.ThrowIfNull(user);
            ArgumentNullException.ThrowIfNull(roles);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName!),
                new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new("firstName", user.FirstName ?? ""),
                new("lastName", user.LastName ?? "")
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

            return tokenString;
        }

        public async Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                {
                    _logger.LogWarning("Token validation failed: Token is null or empty");
                }
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
                    NameClaimType = ClaimTypes.NameIdentifier
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

                var userId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                            principal.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    if (_logger.IsEnabled(LogLevel.Warning))
                    {
                        _logger.LogWarning("Token validation failed: User ID not found in token. Available claims: {Claims}",
                            string.Join(", ", principal.Claims.Select(c => $"{c.Type}:{c.Value}")));
                    }
                    throw new SecurityTokenException("Invalid token claims - missing user ID");
                }

                var isValid = await _tokenStorageService.IsTokenValidAsync(userId, token);

                if (!isValid)
                {
                    if (_logger.IsEnabled(LogLevel.Warning))
                    {
                        _logger.LogWarning("Token validation failed: Token has been revoked for user {UserId}", userId);
                    }
                    throw new SecurityTokenException("Token has been revoked");
                }

                if (_logger.IsEnabled(LogLevel.Debug))
                {
                    _logger.LogDebug("Token validation successful for user {UserId}", userId);
                }

                return principal;
            }
            catch (SecurityTokenExpiredException ex)
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                {
                    _logger.LogWarning("Token validation failed: Token expired - {Message}", ex.Message);
                }
                throw;
            }
            catch (SecurityTokenException ex)
            {
                if (_logger.IsEnabled(LogLevel.Warning))
                {
                    _logger.LogWarning("Token validation failed: Security token error - {Message}", ex.Message);
                }
                throw;
            }
            catch (Exception ex)
            {
                if (_logger.IsEnabled(LogLevel.Error))
                {
                    _logger.LogError(ex, "Token validation failed: Unexpected error");
                }
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
        public bool IsTokenExpiringSoon(string token, int minutesThreshold = 5)
        {
            try
            {
                var expiration = GetTokenExpiration(token);
                return expiration <= DateTime.UtcNow.AddMinutes(minutesThreshold);
            }
            catch
            {
                return true;
            }
        }
    }
}
