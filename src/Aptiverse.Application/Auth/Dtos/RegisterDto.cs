namespace Aptiverse.Application.Auth.Dtos
{
    public record RegisterDto(
        string Username,
        string Email,
        string Password,
        string FirstName,
        string LastName,
        string UserType,
        string? PhoneNumber = null
    );
}
