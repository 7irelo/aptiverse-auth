using Aptiverse.Api.Domain.Models.Resources;
using Aptiverse.Api.Domain.Models.Students;
using Aptiverse.Api.Domain.Models.Teachers;
using Aptiverse.Api.Domain.Models.Tutors;

namespace Aptiverse.Api.Domain.Models.Courses
{
    public class Course
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string SubjectId { get; set; }
        public long? TeacherId { get; set; }
        public long? TutorId { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = "ZAR";
        public string Level { get; set; } // "Beginner", "Intermediate", "Advanced"
        public string ThumbnailUrl { get; set; }
        public string PreviewVideoUrl { get; set; }
        public double Rating { get; set; }
        public int TotalStudents { get; set; }
        public int TotalLessons { get; set; }
        public decimal TotalHours { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual Subject Subject { get; set; }
        public virtual Teacher Teacher { get; set; }
        public virtual Tutor Tutor { get; set; }
        public virtual ICollection<CourseModule> Modules { get; set; }
        public virtual ICollection<CourseEnrollment> Enrollments { get; set; }
        public virtual ICollection<Resource> Resources { get; set; }
    }
}
