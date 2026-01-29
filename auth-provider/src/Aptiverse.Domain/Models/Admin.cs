namespace Aptiverse.Domain.Models
{
    public class Admin
    {
        public long Id { get; set; }
        public required string UserId { get; set; }
        public User? User { get; set; }
        public virtual ICollection<StudentAdmin> StudentAdmins { get; set; } = [];
    }
}
