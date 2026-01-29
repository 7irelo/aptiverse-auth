namespace Aptiverse.Api.Application.ParentStudents.Dtos
{
    public record CreateParentStudentDto
    {
        public long ParentId { get; init; }
        public long StudentId { get; init; }
        public string Relationship { get; init; }
        public bool IsPrimaryContact { get; init; }
    }
}