using Swashbuckle.AspNetCore.Annotations;
using System.Runtime.Serialization;

namespace Aptiverse.Application.Users.Dtos
{
    [DataContract]
    public class UserDto
    {
        [DataMember(Name = "id")][SwaggerSchema(ReadOnly = true)] public string? Id { get; set; }
        [DataMember(Name = "firstName ")] public string? FirstName { get; set; }
        [DataMember(Name = "lastName ")] public string? LastName { get; set; }
        [DataMember(Name = "userName ")] public string? UserName { get; set; }
        [DataMember(Name = "email")] public string? Email { get; set; }
        [DataMember(Name = "phoneNumber")] public string? PhoneNumber { get; set; }
        [DataMember(Name = "password", EmitDefaultValue = false)] public string? Password { get; set; }
        [DataMember(Name = "roles")] public IList<string>? Roles { get; set; }
    }
}
