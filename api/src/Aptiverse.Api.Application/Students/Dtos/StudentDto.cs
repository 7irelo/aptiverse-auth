using Aptiverse.Api.Application.StudentSubjects.Dtos;

namespace Aptiverse.Api.Application.Students.Dtos
{
    public record StudentDto
    {
        public long Id { get; init; }
        public string UserId { get; init; }
        public long? AdminId { get; init; }
        public string Grade { get; init; }
        public string? AdminName { get; init; }
        public virtual ICollection<StudentSubjectDto> StudentSubjects { get; set; }
        public int SubjectCount => StudentSubjects?.Count ?? 0;
    }
}