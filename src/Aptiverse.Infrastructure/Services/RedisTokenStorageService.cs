using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Aptiverse.Infrastructure.Services
{
    public class RedisTokenStorageService : ITokenStorageService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<RedisTokenStorageService> _logger;
        private readonly IDatabase _database;

        public RedisTokenStorageService(
            IConnectionMultiplexer redis,
            ILogger<RedisTokenStorageService> logger)
        {
            _redis = redis;
            _logger = logger;
            _database = redis.GetDatabase();
        }

        public async Task StoreTokenAsync(string userId, string token, DateTime expiry)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be null or empty", nameof(userId));
            if (string.IsNullOrEmpty(token))
                throw new ArgumentException("Token cannot be null or empty", nameof(token));

            try
            {
                var tokenHash = HashToken(token);
                var tokenKey = GetTokenKey(userId, tokenHash);

                var tokenData = new TokenData
                {
                    UserId = userId,
                    TokenHash = tokenHash,
                    Expiry = expiry,
                    CreatedAt = DateTime.UtcNow
                };

                var json = JsonSerializer.Serialize(tokenData);
                var expiryTimeSpan = expiry - DateTime.UtcNow;

                await _database.StringSetAsync(tokenKey, json, expiryTimeSpan);

                // Also add to user's token set for easy management
                var userTokensKey = GetUserTokensKey(userId);
                await _database.SetAddAsync(userTokensKey, tokenHash);

                // Set expiration on user tokens set as well
                await _database.KeyExpireAsync(userTokensKey, expiryTimeSpan);

                _logger.LogInformation("Token stored in Redis for user {UserId} until {Expiry}", userId, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to store token in Redis for user {UserId}", userId);
                throw new InvalidOperationException("Failed to store token", ex);
            }
        }

        public async Task<bool> IsTokenValidAsync(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return false;

            try
            {
                var tokenHash = HashToken(token);
                var tokenKey = GetTokenKey(userId, tokenHash);

                // Check if token exists and get its data
                var tokenJson = await _database.StringGetAsync(tokenKey);

                if (tokenJson.IsNullOrEmpty)
                {
                    _logger.LogDebug("Token not found in Redis for user {UserId}", userId);
                    return false;
                }

                // Token exists and Redis automatically handles expiration
                // If we can retrieve it, it's still valid
                _logger.LogDebug("Token found and valid for user {UserId}", userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking token validity for user {UserId}", userId);
                return false;
            }
        }

        public async Task RevokeTokenAsync(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
                return;

            try
            {
                var tokenHash = HashToken(token);
                var tokenKey = GetTokenKey(userId, tokenHash);
                var userTokensKey = GetUserTokensKey(userId);

                // Remove the token
                await _database.KeyDeleteAsync(tokenKey);

                // Remove from user's token set
                await _database.SetRemoveAsync(userTokensKey, tokenHash);

                _logger.LogInformation("Token revoked for user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to revoke token for user {UserId}", userId);
                throw new InvalidOperationException("Failed to revoke token", ex);
            }
        }

        public async Task RevokeAllUserTokensAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return;

            try
            {
                var userTokensKey = GetUserTokensKey(userId);

                // Get all token hashes for this user
                var tokenHashes = await _database.SetMembersAsync(userTokensKey);

                if (tokenHashes.Any())
                {
                    var keysToDelete = new List<RedisKey> { userTokensKey };

                    foreach (var tokenHash in tokenHashes)
                    {
                        var tokenKey = GetTokenKey(userId, tokenHash.ToString());
                        keysToDelete.Add(tokenKey);
                    }

                    // Delete all tokens in one transaction
                    await _database.KeyDeleteAsync(keysToDelete.ToArray());

                    _logger.LogInformation("Revoked all {TokenCount} tokens for user {UserId}", tokenHashes.Length, userId);
                }
                else
                {
                    _logger.LogDebug("No tokens found to revoke for user {UserId}", userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to revoke all tokens for user {UserId}", userId);
                throw new InvalidOperationException("Failed to revoke all tokens", ex);
            }
        }

        public async Task CleanExpiredTokensAsync(string userId)
        {
            // Redis automatically expires keys, so this is mostly for cleanup of user token sets
            if (string.IsNullOrEmpty(userId))
                return;

            try
            {
                var userTokensKey = GetUserTokensKey(userId);
                var tokenHashes = await _database.SetMembersAsync(userTokensKey);

                var validTokenHashes = new List<RedisValue>();

                foreach (var tokenHash in tokenHashes)
                {
                    var tokenKey = GetTokenKey(userId, tokenHash.ToString());
                    if (await _database.KeyExistsAsync(tokenKey))
                    {
                        validTokenHashes.Add(tokenHash);
                    }
                }

                // If there are expired tokens in the set, update it
                if (validTokenHashes.Count != tokenHashes.Length)
                {
                    await _database.KeyDeleteAsync(userTokensKey);
                    if (validTokenHashes.Any())
                    {
                        await _database.SetAddAsync(userTokensKey, validTokenHashes.ToArray());

                        // Set expiration based on the longest-living token
                        var longestExpiry = await GetLongestTokenExpiryAsync(userId, validTokenHashes);
                        if (longestExpiry.HasValue)
                        {
                            await _database.KeyExpireAsync(userTokensKey, longestExpiry.Value - DateTime.UtcNow);
                        }
                    }

                    _logger.LogDebug("Cleaned expired tokens for user {UserId}", userId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to clean expired tokens for user {UserId}", userId);
            }
        }

        private async Task<DateTime?> GetLongestTokenExpiryAsync(string userId, List<RedisValue> tokenHashes)
        {
            DateTime? longestExpiry = null;

            foreach (var tokenHash in tokenHashes)
            {
                var tokenKey = GetTokenKey(userId, tokenHash.ToString());
                var tokenJson = await _database.StringGetAsync(tokenKey);

                if (!tokenJson.IsNullOrEmpty)
                {
                    try
                    {
                        var tokenData = JsonSerializer.Deserialize<TokenData>(tokenJson.ToString());
                        if (tokenData != null && (!longestExpiry.HasValue || tokenData.Expiry > longestExpiry.Value))
                        {
                            longestExpiry = tokenData.Expiry;
                        }
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning(ex, "Failed to deserialize token data for user {UserId}", userId);
                    }
                }
            }

            return longestExpiry;
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

        private static string GetTokenKey(string userId, string tokenHash)
        {
            return $"token:{userId}:{tokenHash}";
        }

        private static string GetUserTokensKey(string userId)
        {
            return $"user_tokens:{userId}";
        }

        private class TokenData
        {
            public string UserId { get; set; } = string.Empty;
            public string TokenHash { get; set; } = string.Empty;
            public DateTime Expiry { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}