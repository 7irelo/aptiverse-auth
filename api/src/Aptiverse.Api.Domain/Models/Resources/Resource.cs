using Aptiverse.Api.Domain.Models.Courses;
using Aptiverse.Api.Domain.Models.Students;
using Aptiverse.Api.Domain.Models.Teachers;
using Aptiverse.Api.Domain.Models.Tutors;

namespace Aptiverse.Api.Domain.Models.Resources
{
    public class Resource
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SubjectId { get; set; }
        public long? TeacherId { get; set; }
        public long? TutorId { get; set; }
        public long? CourseId { get; set; }
        public string ResourceType { get; set; } // "Worksheet", "Video", "PDF", "Quiz", "Presentation"
        public string S3Key { get; set; }
        public string FileUrl { get; set; }
        public string FileSize { get; set; }
        public string FileFormat { get; set; }
        public decimal Price { get; set; }
        public bool IsFree { get; set; }
        public int DownloadCount { get; set; }
        public double Rating { get; set; }
        public string GradeLevel { get; set; } // "11", "12", "Both"
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Subject Subject { get; set; }
        public virtual Teacher Teacher { get; set; }
        public virtual Tutor Tutor { get; set; }
        public virtual Course Course { get; set; }
        public virtual ICollection<ResourceDownload> Downloads { get; set; }
    }

}
