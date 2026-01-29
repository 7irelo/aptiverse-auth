namespace Aptiverse.Api.Application.AdminStudents.Dtos
{
    public record AdminStudentEnrollmentDto
    {
        public long AdminId { get; init; }
        public string SchoolName { get; init; }
        public long StudentId { get; init; }
        public string StudentName { get; init; }
        public DateTime EnrolledDate { get; init; }
        public string Status { get; init; }
    }
}
