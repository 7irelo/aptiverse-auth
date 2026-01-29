namespace Aptiverse.Api.Domain.Models.Features
{
    public class RoleFeature
    {
        public long Id { get; set; }
        public string RoleName { get; set; }
        public long FeatureId { get; set; }
        public bool IsDefault { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        public virtual Feature Feature { get; set; }
    }
}
