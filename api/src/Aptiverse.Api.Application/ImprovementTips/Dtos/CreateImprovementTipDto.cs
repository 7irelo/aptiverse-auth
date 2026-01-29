namespace Aptiverse.Api.Application.ImprovementTips.Dtos
{
    public record CreateImprovementTipDto
    {
        public long StudentSubjectId { get; init; }
        public string Tip { get; init; }
        public int Priority { get; init; }
    }
}