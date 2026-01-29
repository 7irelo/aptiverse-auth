namespace Aptiverse.Api.Application.UserFeatures.Dtos
{
    public record CreateUserFeatureDto
    {
        public string UserId { get; init; }
        public long FeatureId { get; init; }
        public DateTime? ExpiresAt { get; init; }
        public string GrantType { get; init; }
        public bool IsActive { get; init; }
    }
}