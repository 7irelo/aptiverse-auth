using Swashbuckle.AspNetCore.Annotations;
using System.Runtime.Serialization;

namespace Aptiverse.Application.Users.Dtos
{
    [DataContract]
    public class UserDto
    {
        [DataMember(Name = "id")][SwaggerSchema(ReadOnly = true)] public string Id { get; set; }
        [DataMember(Name = "userName ")] public required string UserName { get; set; }
        [DataMember(Name = "email")] public string? Email { get; set; }
        [DataMember(Name = "phoneNumber")] public string? PhoneNumber { get; set; }
        [DataMember(Name = "password")] public required string Password { get; set; }
    }
}
