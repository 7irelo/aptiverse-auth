namespace Aptiverse.Api.Application.WeeklyStudyHours.Dtos
{
    public record WeeklyStudyHourDto
    {
        public long Id { get; init; }
        public long StudentSubjectId { get; init; }
        public int WeekNumber { get; init; }
        public int Hours { get; init; }
    }
}