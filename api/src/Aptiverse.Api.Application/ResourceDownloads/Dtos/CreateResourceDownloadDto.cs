namespace Aptiverse.Api.Application.ResourceDownloads.Dtos
{
    public record CreateResourceDownloadDto
    {
        public long ResourceId { get; init; }
        public long StudentId { get; init; }
    }
}