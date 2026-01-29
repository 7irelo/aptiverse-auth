using Aptiverse.Api.Domain.Models.Students;

namespace Aptiverse.Api.Domain.Models.Resources
{
    public class ResourceDownload
    {
        public long Id { get; set; }
        public long ResourceId { get; set; }
        public long StudentId { get; set; }
        public DateTime DownloadedAt { get; set; }

        public virtual Resource Resource { get; set; }
        public virtual Student Student { get; set; }
    }
}
