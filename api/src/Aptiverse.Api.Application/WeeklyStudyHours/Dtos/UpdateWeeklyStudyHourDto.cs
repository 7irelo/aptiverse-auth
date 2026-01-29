namespace Aptiverse.Api.Application.WeeklyStudyHours.Dtos
{
    public record UpdateWeeklyStudyHourDto
    {
        public long? StudentSubjectId { get; init; }
        public int? WeekNumber { get; init; }
        public int? Hours { get; init; }
    }
}