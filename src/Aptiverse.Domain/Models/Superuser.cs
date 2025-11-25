namespace Aptiverse.Domain.Models
{
    public class Superuser
    {
        public long Id { get; set; }
        public required string UserId { get; set; }
        public User? User { get; set; }
    }
}
