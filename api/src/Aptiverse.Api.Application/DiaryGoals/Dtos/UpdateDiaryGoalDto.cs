namespace Aptiverse.Api.Application.DiaryGoals.Dtos
{
    public record UpdateDiaryGoalDto
    {
        public string ConnectionType { get; init; }
        public string Notes { get; init; }
    }
}