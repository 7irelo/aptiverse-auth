namespace Aptiverse.Api.Application.StudentPointss.Dtos
{
    public record UpdateStudentPointsDto
    {
        public int? TotalPoints { get; init; }
        public int? AvailablePoints { get; init; }
        public int? UsedPoints { get; init; }
        public int? Level { get; init; }
        public string? CurrentRank { get; init; }
    }
}