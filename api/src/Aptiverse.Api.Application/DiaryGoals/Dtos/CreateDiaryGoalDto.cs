namespace Aptiverse.Api.Application.DiaryGoals.Dtos
{
    public record CreateDiaryGoalDto
    {
        public long DiaryEntryId { get; init; }
        public long GoalId { get; init; }
        public string ConnectionType { get; init; }
        public string Notes { get; init; }
    }
}