namespace Aptiverse.Application.Auth.Dto
{
    public record ResetPasswordDto(string UserId, string ResetToken, string NewPassword);
}
