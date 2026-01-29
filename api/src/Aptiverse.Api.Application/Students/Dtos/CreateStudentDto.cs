namespace Aptiverse.Api.Application.Students.Dtos
{
    public record CreateStudentDto
    {
        public string UserId { get; init; }
        public long? AdminId { get; init; }
        public string Grade { get; init; }
    }
}
