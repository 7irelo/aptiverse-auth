namespace Aptiverse.Api.Application.TutorAvailabilities.Dtos
{
    public record UpdateTutorAvailabilityDto
    {
        public long? TutorId { get; init; }
        public DayOfWeek? DayOfWeek { get; init; }
        public TimeSpan? StartTime { get; init; }
        public TimeSpan? EndTime { get; init; }
        public bool? IsAvailable { get; init; }
    }
}