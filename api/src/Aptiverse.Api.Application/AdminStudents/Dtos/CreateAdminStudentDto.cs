namespace Aptiverse.Api.Application.AdminStudents.Dtos
{
    public record CreateAdminStudentDto
    {
        public long SchoolAdminId { get; init; }
        public long StudentId { get; init; }
        public string EnrollmentStatus { get; init; } = "Active";
    }
}
