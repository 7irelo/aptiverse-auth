using Aptiverse.Api.Domain.Models.Courses;
using Aptiverse.Api.Domain.Models.Resources;
using Aptiverse.Api.Domain.Models.Teachers;

namespace Aptiverse.Api.Domain.Models.Tutors
{
    public class Tutor
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string Qualification { get; set; }
        public string Specialization { get; set; }
        public string Bio { get; set; }
        public decimal HourlyRate { get; set; }
        public int YearsOfExperience { get; set; }
        public string TeachingStyle { get; set; }
        public bool IsVerified { get; set; }
        public double Rating { get; set; }
        public int TotalReviews { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<TutorSubject> TutorSubjects { get; set; }
        public virtual ICollection<TutorStudent> TutorStudents { get; set; }
        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<Resource> Resources { get; set; }
        public virtual ICollection<TutorAvailability> Availabilities { get; set; }
    }
}
