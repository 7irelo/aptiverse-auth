namespace Aptiverse.Api.Application.TeacherStudents.Dtos
{
    public record TeacherStudentDto
    {
        public long Id { get; init; }
        public long TeacherId { get; init; }
        public long StudentId { get; init; }
        public DateTime AssignedDate { get; init; }
        public bool IsActive { get; init; }
    }
}