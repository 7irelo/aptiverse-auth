namespace Aptiverse.Api.Application.GoalMilestones.Dtos
{
    public record CreateGoalMilestoneDto
    {
        public long GoalId { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public decimal TargetValue { get; init; }
        public int RewardPoints { get; init; }
    }
}