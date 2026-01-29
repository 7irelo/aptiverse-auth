namespace Aptiverse.Api.Application.StudentRewards.Dtos
{
    public record UpdateStudentRewardDto
    {
        public long? StudentId { get; init; }
        public long? RewardId { get; init; }
        public long? GoalId { get; init; }
        public DateTime? EarnedAt { get; init; }
        public DateTime? ExpiresAt { get; init; }
        public string? Status { get; init; }
        public int? PointsEarned { get; init; }
        public string? AchievementContext { get; init; }
    }
}