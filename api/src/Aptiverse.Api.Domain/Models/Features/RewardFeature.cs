using Aptiverse.Api.Domain.Models.Rewards;

namespace Aptiverse.Api.Domain.Models.Features
{
    public class RewardFeature
    {
        public long Id { get; set; }
        public long RewardId { get; set; }
        public long FeatureId { get; set; }
        public int DurationDays { get; set; } // How long the feature access lasts
        public int FeatureWeight { get; set; } // How valuable this feature is (1-100)

        // Navigation properties
        public virtual Reward Reward { get; set; }
        public virtual Feature Feature { get; set; }
    }
}
