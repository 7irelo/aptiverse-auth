namespace Aptiverse.Api.Application.ParentStudents.Dtos
{
    public record ParentStudentDto
    {
        public long Id { get; init; }
        public long ParentId { get; init; }
        public long StudentId { get; init; }
        public string Relationship { get; init; }
        public bool IsPrimaryContact { get; init; }
    }
}