namespace Aptiverse.Api.Application.Goals.Dtos
{
    public record GoalDto
    {
        public long Id { get; init; }
        public long StudentId { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public string GoalType { get; init; }
        public string SubjectId { get; init; }
        public decimal TargetValue { get; init; }
        public decimal CurrentValue { get; init; }
        public string Unit { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime TargetDate { get; init; }
        public string Status { get; init; }
        public int Priority { get; init; }
        public int DifficultyWeight { get; init; }
        public decimal ProgressPercentage { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime UpdatedAt { get; init; }
    }
}