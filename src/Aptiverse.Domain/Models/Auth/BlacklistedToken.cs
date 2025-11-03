using System.ComponentModel.DataAnnotations;

namespace Aptiverse.Domain.Models.Auth
{
    public class BlacklistedToken
    {
        [Key]
        [MaxLength(44)]
        public string TokenHash { get; set; } = string.Empty;

        public DateTime Expiry { get; set; }
        public DateTime BlacklistedAt { get; set; }
    }
}
