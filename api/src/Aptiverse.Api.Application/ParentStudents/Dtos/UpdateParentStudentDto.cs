namespace Aptiverse.Api.Application.ParentStudents.Dtos
{
    public record UpdateParentStudentDto
    {
        public string Relationship { get; init; }
        public bool IsPrimaryContact { get; init; }
    }
}