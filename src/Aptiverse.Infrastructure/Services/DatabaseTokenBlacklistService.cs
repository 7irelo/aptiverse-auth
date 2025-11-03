using Aptiverse.Domain.Models.Auth;
using Aptiverse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace Aptiverse.Infrastructure.Services
{
    public class DatabaseTokenBlacklistService : ITokenBlacklistService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<DatabaseTokenBlacklistService> _logger;

        public DatabaseTokenBlacklistService(ApplicationDbContext context, ILogger<DatabaseTokenBlacklistService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
                return true;

            var tokenHash = HashToken(token);
            var blacklistedToken = await _context.BlacklistedTokens
                .FirstOrDefaultAsync(bt => bt.TokenHash == tokenHash && bt.Expiry > DateTime.Now);

            return blacklistedToken != null;
        }

        public async Task BlacklistTokenAsync(string token, DateTime expiry)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token cannot be null or empty", nameof(token));

            var tokenHash = HashToken(token);

            var existing = await _context.BlacklistedTokens
                .FirstOrDefaultAsync(bt => bt.TokenHash == tokenHash);

            if (existing != null)
            {
                existing.Expiry = expiry;   
                existing.BlacklistedAt = DateTime.Now;
            }
            else
            {
                var blacklistedToken = new BlacklistedToken
                {
                    TokenHash = tokenHash,
                    Expiry = expiry,
                    BlacklistedAt = DateTime.Now
                };
                _context.BlacklistedTokens.Add(blacklistedToken);
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Token blacklisted until {Expiry}", expiry);
        }

        public async Task CleanExpiredTokensAsync()
        {
            var expired = DateTime.Now;
            var expiredTokens = await _context.BlacklistedTokens
                .Where(bt => bt.Expiry <= expired)
                .ToListAsync();

            if (expiredTokens.Any())
            {
                _context.BlacklistedTokens.RemoveRange(expiredTokens);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Cleaned {Count} expired blacklisted tokens", expiredTokens.Count);
            }
        }

        private static string HashToken(string token)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));
            return Convert.ToBase64String(hashBytes);
        }
    }
}
