namespace Aptiverse.Api.Application.CourseEnrollments.Dtos
{
    public record CourseEnrollmentDto
    {
        public long Id { get; init; }
        public long CourseId { get; init; }
        public long StudentId { get; init; }
        public DateTime EnrolledAt { get; init; }
        public decimal AmountPaid { get; init; }
        public string PaymentStatus { get; init; }
        public decimal Progress { get; init; }
        public DateTime? CompletedAt { get; init; }
    }
}