namespace Aptiverse.Api.Application.GoalMilestones.Dtos
{
    public record UpdateGoalMilestoneDto
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public decimal TargetValue { get; init; }
        public bool IsCompleted { get; init; }
        public int RewardPoints { get; init; }
    }
}