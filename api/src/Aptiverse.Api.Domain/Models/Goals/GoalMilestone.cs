namespace Aptiverse.Api.Domain.Models.Goals
{
    public class GoalMilestone
    {
        public long Id { get; set; }
        public long GoalId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal TargetValue { get; set; }
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int RewardPoints { get; set; }

        // Navigation properties
        public virtual Goal Goal { get; set; }
    }
}
