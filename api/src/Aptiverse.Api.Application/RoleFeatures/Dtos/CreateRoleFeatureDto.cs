namespace Aptiverse.Api.Application.RoleFeatures.Dtos
{
    public record CreateRoleFeatureDto
    {
        public string RoleName { get; init; }
        public long FeatureId { get; init; }
        public bool IsDefault { get; init; }
    }
}