namespace Aptiverse.Api.Application.KnowledgeGaps.Dtos
{
    public record UpdateKnowledgeGapDto
    {
        public long? StudentSubjectId { get; init; }
        public string? Concept { get; init; }
        public string? Severity { get; init; }
        public DateTime? LastTested { get; init; }
    }
}