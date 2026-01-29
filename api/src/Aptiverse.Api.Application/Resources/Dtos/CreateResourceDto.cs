namespace Aptiverse.Api.Application.Resources.Dtos
{
    public record CreateResourceDto
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public string SubjectId { get; init; }
        public long? TeacherId { get; init; }
        public long? TutorId { get; init; }
        public long? CourseId { get; init; }
        public string ResourceType { get; init; }
        public string S3Key { get; init; }
        public string FileUrl { get; init; }
        public string FileSize { get; init; }
        public string FileFormat { get; init; }
        public decimal Price { get; init; }
        public bool IsFree { get; init; }
        public string GradeLevel { get; init; }
        public bool IsApproved { get; init; }
    }
}