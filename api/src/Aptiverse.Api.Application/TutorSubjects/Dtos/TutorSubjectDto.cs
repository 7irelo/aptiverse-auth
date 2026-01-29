namespace Aptiverse.Api.Application.TutorSubjects.Dtos
{
    public record TutorSubjectDto
    {
        public long Id { get; init; }
        public long TutorId { get; init; }
        public string SubjectId { get; init; }
        public int ProficiencyLevel { get; init; }
        public decimal CustomHourlyRate { get; init; }
    }
}