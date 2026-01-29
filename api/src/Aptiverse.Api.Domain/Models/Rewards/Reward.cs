using Aptiverse.Api.Domain.Models.Features;
using Aptiverse.Api.Domain.Models.Goals;

namespace Aptiverse.Api.Domain.Models.Rewards
{
    public class Reward
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string RewardType { get; set; } // "FeatureAccess", "Points", "Badge", "Discount", "Recognition"
        public int PointsCost { get; set; }
        public int DifficultyTier { get; set; } // 1-5 scale
        public bool IsActive { get; set; } = true;
        public int StockQuantity { get; set; } = -1; // -1 for unlimited
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<RewardFeature> RewardFeatures { get; set; }
        public virtual ICollection<StudentReward> StudentRewards { get; set; }
        public virtual ICollection<Goal> ApplicableGoals { get; set; }
    }
}
