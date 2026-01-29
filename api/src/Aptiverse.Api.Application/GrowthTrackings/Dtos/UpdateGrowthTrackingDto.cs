namespace Aptiverse.Api.Application.GrowthTrackings.Dtos
{
    public record UpdateGrowthTrackingDto
    {
        public decimal AcademicGrowth { get; init; }
        public decimal StudyHabitGrowth { get; init; }
        public decimal EmotionalGrowth { get; init; }
        public string GrowthFactors { get; init; }
        public string AreasForImprovement { get; init; }
    }
}