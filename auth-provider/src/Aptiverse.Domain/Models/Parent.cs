namespace Aptiverse.Domain.Models
{
    public class Parent
    {
        public long Id { get; set; }
        public required string UserId { get; set; }
        public User? User { get; set; }
        public virtual ICollection<StudentParent> StudentParents { get; set; } = [];
    }
}
