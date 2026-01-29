namespace Aptiverse.Api.Domain.Models.Features
{
    public class UserFeature
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public long FeatureId { get; set; }
        public DateTime GrantedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string GrantType { get; set; } // "Purchase", "Reward", "Trial", "AdminGrant"
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual Feature Feature { get; set; }
    }
}
