using System;
using System.Collections.Generic;
using System.Text;

namespace Aptiverse.Api.Infrastructure.Caching
{
    public interface IRedisCacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default);
        Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
        Task<IEnumerable<string>> GetKeysAsync(string pattern, CancellationToken cancellationToken = default);
        Task ClearByPatternAsync(string pattern, CancellationToken cancellationToken = default);
        Task ClearAllAsync(CancellationToken cancellationToken = default);
    }
}
