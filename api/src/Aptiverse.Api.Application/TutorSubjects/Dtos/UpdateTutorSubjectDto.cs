namespace Aptiverse.Api.Application.TutorSubjects.Dtos
{
    public record UpdateTutorSubjectDto
    {
        public long? TutorId { get; init; }
        public string? SubjectId { get; init; }
        public int? ProficiencyLevel { get; init; }
        public decimal? CustomHourlyRate { get; init; }
    }
}