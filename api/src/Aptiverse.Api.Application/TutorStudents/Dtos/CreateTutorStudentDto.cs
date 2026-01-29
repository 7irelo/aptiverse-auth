namespace Aptiverse.Api.Application.TutorStudents.Dtos
{
    public record CreateTutorStudentDto
    {
        public long TutorId { get; init; }
        public long StudentId { get; init; }
        public DateTime StartedDate { get; init; }
        public bool IsActive { get; init; } = true;
        public int SessionsPerWeek { get; init; }
    }
}