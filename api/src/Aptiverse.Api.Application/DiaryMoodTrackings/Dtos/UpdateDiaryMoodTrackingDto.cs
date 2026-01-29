namespace Aptiverse.Api.Application.DiaryMoodTrackings.Dtos
{
    public record UpdateDiaryMoodTrackingDto
    {
        public string OverallMood { get; init; }
        public int EnergyLevel { get; init; }
        public int StressLevel { get; init; }
        public int MotivationLevel { get; init; }
        public string FactorsAffectingMood { get; init; }
        public string Notes { get; init; }
    }
}