namespace Aptiverse.Api.Application.CourseEnrollments.Dtos
{
    public record UpdateCourseEnrollmentDto
    {
        public decimal AmountPaid { get; init; }
        public string PaymentStatus { get; init; }
        public decimal Progress { get; init; }
        public DateTime? CompletedAt { get; init; }
    }
}