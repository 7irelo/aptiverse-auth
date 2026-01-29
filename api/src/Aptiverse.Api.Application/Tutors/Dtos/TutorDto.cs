namespace Aptiverse.Api.Application.Tutors.Dtos
{
    public record TutorDto
    {
        public long Id { get; init; }
        public string UserId { get; init; }
        public string Qualification { get; init; }
        public string Specialization { get; init; }
        public string Bio { get; init; }
        public decimal HourlyRate { get; init; }
        public int YearsOfExperience { get; init; }
        public string TeachingStyle { get; init; }
        public bool IsVerified { get; init; }
        public double Rating { get; init; }
        public int TotalReviews { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}