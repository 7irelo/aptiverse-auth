namespace Aptiverse.Api.Application.GoalMilestones.Dtos
{
    public record GoalMilestoneDto
    {
        public long Id { get; init; }
        public long GoalId { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public decimal TargetValue { get; init; }
        public bool IsCompleted { get; init; }
        public DateTime? CompletedAt { get; init; }
        public int RewardPoints { get; init; }
    }
}