using Aptiverse.Api.Domain.Models.Goals;
using Aptiverse.Api.Domain.Models.Students;

namespace Aptiverse.Api.Domain.Models.Rewards
{
    public class StudentReward
    {
        public long Id { get; set; }
        public long StudentId { get; set; }
        public long RewardId { get; set; }
        public long? GoalId { get; set; } // Optional: which goal earned this reward
        public DateTime EarnedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string Status { get; set; } // "Active", "Used", "Expired"
        public int PointsEarned { get; set; }
        public string AchievementContext { get; set; } // Description of how it was earned

        // Navigation properties
        public virtual Student Student { get; set; }
        public virtual Reward Reward { get; set; }
        public virtual Goal Goal { get; set; }
    }
}
