namespace Aptiverse.Api.Application.Admins.Dtos
{
    public record AdminSummaryDto
    {
        public long Id { get; init; }
        public string SchoolName { get; init; }
        public string SchoolCode { get; init; }
        public int TotalStudents { get; init; }
        public int TotalTeachers { get; init; }
        public int ActiveStudents { get; init; }
        public int ActiveTeachers { get; init; }
        public DateTime LastEnrollmentDate { get; init; }
    }
}
