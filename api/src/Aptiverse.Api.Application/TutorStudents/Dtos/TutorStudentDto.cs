namespace Aptiverse.Api.Application.TutorStudents.Dtos
{
    public record TutorStudentDto
    {
        public long Id { get; init; }
        public long TutorId { get; init; }
        public long StudentId { get; init; }
        public DateTime StartedDate { get; init; }
        public bool IsActive { get; init; }
        public int SessionsPerWeek { get; init; }
    }
}