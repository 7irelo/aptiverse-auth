namespace Aptiverse.Api.Application.Teachers.Dtos
{
    public record UpdateTeacherDto
    {
        public string? UserId { get; init; }
        public string? Qualification { get; init; }
        public string? Specialization { get; init; }
        public int? YearsOfExperience { get; init; }
        public string? Bio { get; init; }
        public decimal? HourlyRate { get; init; }
        public bool? IsVerified { get; init; }
    }
}