namespace Aptiverse.Api.Application.TeacherSubjects.Dtos
{
    public record CreateTeacherSubjectDto
    {
        public long TeacherId { get; init; }
        public string SubjectId { get; init; }
        public int ProficiencyLevel { get; init; }
    }
}