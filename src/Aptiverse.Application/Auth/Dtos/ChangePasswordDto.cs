namespace Aptiverse.Application.Auth.Dtos
{
    public record ChangePasswordDto(string CurrentPassword, string NewPassword);
}
