namespace Aptiverse.Infrastructure.Services
{
    public interface ITokenStorageService
    {
        Task StoreTokenAsync(string userId, string token, DateTime expiry);
        Task<bool> IsTokenValidAsync(string userId, string token);
        Task RevokeTokenAsync(string userId, string token);
        Task RevokeAllUserTokensAsync(string userId);
    }
}
