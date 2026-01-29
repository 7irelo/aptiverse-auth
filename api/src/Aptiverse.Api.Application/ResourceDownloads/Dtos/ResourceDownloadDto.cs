namespace Aptiverse.Api.Application.ResourceDownloads.Dtos
{
    public record ResourceDownloadDto
    {
        public long Id { get; init; }
        public long ResourceId { get; init; }
        public long StudentId { get; init; }
        public DateTime DownloadedAt { get; init; }
    }
}