using Aptiverse.Api.Domain.Models.Students;

namespace Aptiverse.Api.Domain.Models.Admins
{
    public class AdminStudent
    {
        public long Id { get; set; }
        public long SchoolAdminId { get; set; }
        public long StudentId { get; set; }
        public DateTime EnrolledDate { get; set; } = DateTime.UtcNow;
        public string EnrollmentStatus { get; set; } = "Active";

        public virtual Admin SchoolAdmin { get; set; }
        public virtual Student Student { get; set; }
    }
}
