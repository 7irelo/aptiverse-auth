using Aptiverse.Application.Auth.Dtos;
using Aptiverse.Application.Users.Dtos;
using Aptiverse.Core.Dtos;
using System.Security.Claims;

namespace Aptiverse.Application.Auth.Services
{
    public interface IAuthService
    {
        Task<UserDto> GetCurrentUserAsync(ClaimsPrincipal loggedInUser);
        Task RegisterUserAsync(RegisterDto registerDto);
        Task<TokenDto<UserDto>> LoginUserAsync(LoginDto dto);
        Task<TokenDto<UserDto>> RefreshTokenAsync(ClaimsPrincipal loggedInUser);
        Task<TokenValidDto> ValidateTokenAsync(ValidateTokenDto validateTokenDto);
        Task<object> ChangePasswordAsync(ClaimsPrincipal loggedInUser, ChangePasswordDto changePasswordDto);
        Task<object> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto);
        Task<object> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
        Task<object> LogoutUserAsync(ClaimsPrincipal loggedInUser);
        Task ConfirmEmail(string userId, string token);
    }
}
