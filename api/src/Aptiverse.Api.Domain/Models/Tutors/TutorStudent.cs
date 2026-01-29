using Aptiverse.Api.Domain.Models.Students;

namespace Aptiverse.Api.Domain.Models.Tutors
{
    public class TutorStudent
    {
        public long Id { get; set; }
        public long TutorId { get; set; }
        public long StudentId { get; set; }
        public DateTime StartedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public int SessionsPerWeek { get; set; }

        public virtual Tutor Tutor { get; set; }
        public virtual Student Student { get; set; }
    }
}
