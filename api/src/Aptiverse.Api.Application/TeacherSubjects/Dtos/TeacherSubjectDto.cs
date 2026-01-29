namespace Aptiverse.Api.Application.TeacherSubjects.Dtos
{
    public record TeacherSubjectDto
    {
        public long Id { get; init; }
        public long TeacherId { get; init; }
        public string SubjectId { get; init; }
        public int ProficiencyLevel { get; init; }
    }
}