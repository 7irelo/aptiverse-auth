namespace Aptiverse.Infrastructure.Services
{
    public interface ITokenBlacklistService
    {
        Task<bool> IsTokenBlacklistedAsync(string token);
        Task BlacklistTokenAsync(string token, DateTime expiry);
        Task CleanExpiredTokensAsync();
    }
}
