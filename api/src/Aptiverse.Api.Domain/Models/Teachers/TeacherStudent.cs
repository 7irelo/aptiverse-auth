using Aptiverse.Api.Domain.Models.Students;

namespace Aptiverse.Api.Domain.Models.Teachers
{
    public class TeacherStudent
    {
        public long Id { get; set; }
        public long TeacherId { get; set; }
        public long StudentId { get; set; }
        public DateTime AssignedDate { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual Teacher Teacher { get; set; }
        public virtual Student Student { get; set; }
    }
}
