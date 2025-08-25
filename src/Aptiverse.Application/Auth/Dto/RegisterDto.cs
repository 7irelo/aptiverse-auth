namespace Aptiverse.Application.Auth.Dto
{
    public record RegisterDto(string Username, string Email, string Password, string? PhoneNumber = null);
}
