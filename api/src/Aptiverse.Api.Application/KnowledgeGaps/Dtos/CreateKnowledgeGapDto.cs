namespace Aptiverse.Api.Application.KnowledgeGaps.Dtos
{
    public record CreateKnowledgeGapDto
    {
        public long StudentSubjectId { get; init; }
        public string Concept { get; init; }
        public string Severity { get; init; }
        public DateTime LastTested { get; init; }
    }
}