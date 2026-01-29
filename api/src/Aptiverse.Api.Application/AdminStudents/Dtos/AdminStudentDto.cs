namespace Aptiverse.Api.Application.AdminStudents.Dtos
{
    public record AdminStudentDto
    {
        public long Id { get; init; }
        public long SchoolAdminId { get; init; }
        public long StudentId { get; init; }
        public DateTime EnrolledDate { get; init; }
        public string EnrollmentStatus { get; init; }
        public string? SchoolName { get; init; }
        public string? StudentName { get; init; }
        public string? StudentGrade { get; init; }
    }
}
