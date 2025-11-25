using System.Runtime.Serialization;

namespace Aptiverse.Core.Dtos
{
    public class ValidateTokenDto
    {
        [DataMember(Name = "token")] public string Token { get; set; } = string.Empty;
    }
}
