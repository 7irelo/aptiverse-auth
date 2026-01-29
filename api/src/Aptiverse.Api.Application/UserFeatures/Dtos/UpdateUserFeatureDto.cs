namespace Aptiverse.Api.Application.UserFeatures.Dtos
{
    public record UpdateUserFeatureDto
    {
        public DateTime? ExpiresAt { get; init; }
        public bool IsActive { get; init; }
    }
}