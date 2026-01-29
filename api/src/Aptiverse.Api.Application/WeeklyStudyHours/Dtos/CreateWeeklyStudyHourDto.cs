namespace Aptiverse.Api.Application.WeeklyStudyHours.Dtos
{
    public record CreateWeeklyStudyHourDto
    {
        public long StudentSubjectId { get; init; }
        public int WeekNumber { get; init; }
        public int Hours { get; init; }
    }
}