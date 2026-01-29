namespace Aptiverse.Api.Application.KnowledgeGaps.Dtos
{
    public record KnowledgeGapDto
    {
        public long Id { get; init; }
        public long StudentSubjectId { get; init; }
        public string Concept { get; init; }
        public string Severity { get; init; }
        public DateTime LastTested { get; init; }
    }
}