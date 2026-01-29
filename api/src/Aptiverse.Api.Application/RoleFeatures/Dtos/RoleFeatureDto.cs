namespace Aptiverse.Api.Application.RoleFeatures.Dtos
{
    public record RoleFeatureDto
    {
        public long Id { get; init; }
        public string RoleName { get; init; }
        public long FeatureId { get; init; }
        public bool IsDefault { get; init; }
        public DateTime AssignedAt { get; init; }
    }
}