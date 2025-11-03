using System.Runtime.Serialization;

namespace Aptiverse.Core.Dtos
{
    [DataContract]
    public class TokenDto<T>
    {
        [DataMember(Name = "token")] public string Token { get; set; }
        [DataMember(Name = "expires")] public DateTime Expires { get; set; }
        [DataMember(Name = "user")] public T User { get; set; }
        [DataMember(Name = "message")] public string Message { get; set; }
    }
}
