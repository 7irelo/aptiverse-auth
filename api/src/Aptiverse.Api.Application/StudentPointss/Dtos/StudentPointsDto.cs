namespace Aptiverse.Api.Application.StudentPointss.Dtos
{
    public record StudentPointsDto
    {
        public long Id { get; init; }
        public long StudentId { get; init; }
        public int TotalPoints { get; init; }
        public int AvailablePoints { get; init; }
        public int UsedPoints { get; init; }
        public int Level { get; init; }
        public string CurrentRank { get; init; }
        public DateTime LastUpdated { get; init; }
    }
}