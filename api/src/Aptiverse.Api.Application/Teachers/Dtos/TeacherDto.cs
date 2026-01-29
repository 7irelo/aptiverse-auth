namespace Aptiverse.Api.Application.Teachers.Dtos
{
    public record TeacherDto
    {
        public long Id { get; init; }
        public string UserId { get; init; }
        public string Qualification { get; init; }
        public string Specialization { get; init; }
        public int YearsOfExperience { get; init; }
        public string Bio { get; init; }
        public decimal HourlyRate { get; init; }
        public bool IsVerified { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}