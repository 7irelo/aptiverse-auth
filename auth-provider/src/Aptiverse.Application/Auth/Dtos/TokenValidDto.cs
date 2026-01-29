using System.Collections;
using System.Runtime.Serialization;

namespace Aptiverse.Application.Auth.Dtos
{
    [DataContract]
    public class TokenValidDto
    {
        [DataMember(Name = "valid")] public bool Valid { get; set; }
        [DataMember(Name = "claims")] public IEnumerable? Claims { get; set; }
    }
}
