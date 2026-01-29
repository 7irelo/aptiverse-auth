namespace Aptiverse.Api.Domain.Models.Parents
{
    public class Parent
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string? ContactNumber { get; set; }
        public string? Address { get; set; }
        public string? Occupation { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public virtual ICollection<ParentStudent>? ParentStudents { get; set; }
    }
}
