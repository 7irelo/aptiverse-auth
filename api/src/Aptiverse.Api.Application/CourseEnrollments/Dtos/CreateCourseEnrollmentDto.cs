namespace Aptiverse.Api.Application.CourseEnrollments.Dtos
{
    public record CreateCourseEnrollmentDto
    {
        public long CourseId { get; init; }
        public long StudentId { get; init; }
        public decimal AmountPaid { get; init; }
        public string PaymentStatus { get; init; }
        public decimal Progress { get; init; }
    }
}