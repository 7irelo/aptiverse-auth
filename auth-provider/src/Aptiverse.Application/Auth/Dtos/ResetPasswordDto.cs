namespace Aptiverse.Application.Auth.Dtos
{
    public record ResetPasswordDto(string UserId, string ResetToken, string NewPassword);
}
