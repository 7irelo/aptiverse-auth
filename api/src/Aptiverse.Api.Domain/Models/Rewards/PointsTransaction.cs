using Aptiverse.Api.Domain.Models.Goals;

namespace Aptiverse.Api.Domain.Models.Rewards
{
    public class PointsTransaction
    {
        public long Id { get; set; }
        public long StudentPointsId { get; set; }
        public int Points { get; set; }
        public string TransactionType { get; set; } // "Earned", "Spent", "Bonus", "Penalty"
        public string Source { get; set; } // "GoalCompletion", "Milestone", "DailyLogin", "RewardRedemption"
        public long? RelatedGoalId { get; set; }
        public long? RelatedRewardId { get; set; }
        public string Description { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual StudentPoints StudentPoints { get; set; }
        public virtual Goal RelatedGoal { get; set; }
        public virtual Reward RelatedReward { get; set; }
    }
}
