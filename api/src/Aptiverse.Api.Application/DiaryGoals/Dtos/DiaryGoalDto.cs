namespace Aptiverse.Api.Application.DiaryGoals.Dtos
{
    public record DiaryGoalDto
    {
        public long Id { get; init; }
        public long DiaryEntryId { get; init; }
        public long GoalId { get; init; }
        public string ConnectionType { get; init; }
        public string Notes { get; init; }
    }
}