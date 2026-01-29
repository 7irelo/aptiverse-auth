using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Text.Json;

namespace Aptiverse.Api.Infrastructure.Caching
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _database;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
        {
            _database = redis.GetDatabase();
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var cachedValue = await _database.StringGetAsync(key);
                if (!cachedValue.HasValue)
                {
                    _logger.LogDebug("Cache miss for key: {Key}", key);
                    return default;
                }

                _logger.LogDebug("Cache hit for key: {Key}", key);
                return JsonSerializer.Deserialize<T>(cachedValue.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache for key: {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            try
            {
                var serializedValue = JsonSerializer.Serialize(value);

                // Option 1: Use the correct overload
                if (expiry.HasValue)
                {
                    await _database.StringSetAsync(key, serializedValue, expiry.Value);
                }
                else
                {
                    await _database.StringSetAsync(key, serializedValue);
                }

                // Option 2: Use the explicit parameter (alternative syntax)
                // await _database.StringSetAsync(key, serializedValue, expiry: expiry);

                _logger.LogDebug("Cache set for key: {Key} with expiry: {Expiry}", key, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache for key: {Key}", key);
            }
        }

        public async Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var result = await _database.KeyDeleteAsync(key);
                if (result)
                {
                    _logger.LogDebug("Cache removed for key: {Key}", key);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache for key: {Key}", key);
                return false;
            }
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                return await _database.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking cache existence for key: {Key}", key);
                return false;
            }
        }

        public async Task<IEnumerable<string>> GetKeysAsync(string pattern, CancellationToken cancellationToken = default)
        {
            var keys = new List<string>();
            try
            {
                var endpoints = _database.Multiplexer.GetEndPoints();
                var server = _database.Multiplexer.GetServer(endpoints.First());

                await foreach (var key in server.KeysAsync(pattern: pattern))
                {
                    keys.Add(key.ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting keys for pattern: {Pattern}", pattern);
            }

            return keys;
        }

        public async Task ClearByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            try
            {
                var keys = await GetKeysAsync(pattern, cancellationToken);
                foreach (var key in keys)
                {
                    await _database.KeyDeleteAsync(key);
                }
                _logger.LogDebug("Cleared cache for pattern: {Pattern} (removed {Count} keys)", pattern, keys.Count());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing cache for pattern: {Pattern}", pattern);
            }
        }

        public async Task ClearAllAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var endpoints = _database.Multiplexer.GetEndPoints();
                var server = _database.Multiplexer.GetServer(endpoints.First());
                await server.FlushDatabaseAsync();
                _logger.LogDebug("Cleared all cache");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing all cache");
            }
        }
    }
}