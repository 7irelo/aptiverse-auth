namespace Aptiverse.Api.Application.GrowthTrackings.Dtos
{
    public record CreateGrowthTrackingDto
    {
        public long StudentId { get; init; }
        public DateTime TrackingDate { get; init; }
        public decimal AcademicGrowth { get; init; }
        public decimal StudyHabitGrowth { get; init; }
        public decimal EmotionalGrowth { get; init; }
        public string GrowthFactors { get; init; }
        public string AreasForImprovement { get; init; }
    }
}