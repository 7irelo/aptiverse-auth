namespace Aptiverse.Api.Application.StudentPointss.Dtos
{
    public record CreateStudentPointsDto
    {
        public long StudentId { get; init; }
        public int TotalPoints { get; init; }
        public int AvailablePoints { get; init; }
        public int UsedPoints { get; init; }
        public int Level { get; init; } = 1;
        public string CurrentRank { get; init; } = "Beginner";
    }
}