using Aptiverse.Application.Auth.Dtos;
using Aptiverse.Application.Users.Dtos;
using Aptiverse.Core.Dtos;
using Aptiverse.Core.Exceptions;
using Aptiverse.Core.Services;
using Aptiverse.Domain.Models;
using Aptiverse.Infrastructure.Data;
using Aptiverse.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Aptiverse.Application.Auth.Services
{
    public class AuthService(
        UserManager<User> userManager,
        ApplicationDbContext dbContext,
        IMapper mapper,
        ITokenProvider tokenProvider,
        IConfiguration config,
        ITokenStorageService tokenStorageService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthService> logger,
        IEmailSender emailSender,
        IHttpClientFactory httpClientFactory) : IAuthService
    {
        private readonly UserManager<User> _userManager = userManager;
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        private readonly IConfiguration _config = config;
        private readonly ITokenStorageService _tokenStorageService = tokenStorageService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ILogger<AuthService> _logger = logger;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        #region Get Current User
        public async Task<UserDto> GetCurrentUserAsync(ClaimsPrincipal loggedInUser)
        {
            if (loggedInUser?.Identity?.IsAuthenticated != true)
            {
                throw new AuthenticationException("User is not authenticated");
            }

            try
            {
                var user = await _userManager.GetUserAsync(loggedInUser) ?? throw new UserRetrievalException("User account not found");
                var roles = await _userManager.GetRolesAsync(user);

                return new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = [.. roles]
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCurrentUserAsync: Unexpected error");
                throw;
            }
        }
        #endregion

        #region Register User
        public async Task RegisterUserAsync(RegisterDto registerDto)
        {
            var user = _mapper.Map<User>(registerDto);
            user.CreatedAt = DateTime.UtcNow;

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                throw new UserRegistrationException($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            try
            {
                var roleResult = await _userManager.AddToRoleAsync(user, registerDto.UserType);

                if (!roleResult.Succeeded)
                {
                    _logger.LogWarning("Could not add user to role: {Errors}",
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }

                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = $"https://aptiverse.co.za/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(emailConfirmationToken)}";

                var templateData = new
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    UserName = user.UserName,
                    Email = user.Email,
                    UserType = registerDto.UserType,
                    ConfirmationLink = confirmationLink
                };

                await _emailSender.SendTemplateEmailAsync(
                    user.Email ?? string.Empty,
                    "Confirm Your Email - Aptiverse",
                    "email_confirmation",
                    templateData);
            }
            catch (Exception ex) when (ex is not UserRegistrationException and not RoleAssignmentException)
            {
                await _userManager.DeleteAsync(user);
                throw new UserRegistrationException("An unexpected error occurred during user registration", ex);
            }
        }
        #endregion

        #region Login User
        public async Task<TokenDto<UserDto>> LoginUserAsync(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email)
                ?? throw new AuthenticationException("Invalid email or password");

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid)
            {
                throw new AuthenticationException("Invalid email or password");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var token = await _tokenProvider.GenerateJwtTokenAsync(user, roles);

            return new TokenDto<UserDto>
            {
                Token = token.ToString(),
                Expires = DateTime.Now.AddHours(Convert.ToDouble(_config["Jwt:ExpireHours"] ?? "4")),
                User = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                },
                Message = "Login successful"
            };
        }
        #endregion

        #region Refresh Token
        public async Task<TokenDto<UserDto>> RefreshTokenAsync(ClaimsPrincipal loggedInUser)
        {
            if (loggedInUser?.Identity?.IsAuthenticated != true)
            {
                throw new TokenRefreshException("User is not authenticated");
            }

            var userId = loggedInUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new TokenRefreshException("Invalid token claims");
            }

            var user = await _userManager.FindByIdAsync(userId)
                ?? throw new TokenRefreshException("User not found");

            try
            {
                var roles = await _userManager.GetRolesAsync(user);
                var token = await _tokenProvider.GenerateJwtTokenAsync(user, roles);

                return new TokenDto<UserDto>
                {
                    Token = token,
                    Expires = DateTime.Now.AddHours(Convert.ToDouble(_config["Jwt:ExpireHours"] ?? "4")),
                    User = new UserDto
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                    },
                    Message = "Token Refresh Successful"
                };
            }
            catch (Exception ex) when (ex is not TokenRefreshException)
            {
                throw new TokenRefreshException("Failed to generate new token", ex);
            }
        }
        #endregion

        #region Validate Token
        public async Task<TokenValidDto> ValidateTokenAsync(ValidateTokenDto validateTokenDto)
        {
            if (validateTokenDto == null)
            {
                throw new TokenValidationException("Token validation request is null");
            }

            if (string.IsNullOrWhiteSpace(validateTokenDto.Token))
            {
                throw new TokenValidationException("Token is required");
            }

            try
            {
                var principal = await _tokenProvider.ValidateTokenAsync(validateTokenDto.Token);

                if (principal != null)
                {
                    return new TokenValidDto
                    {
                        Valid = true,
                        Claims = principal.Claims.ToDictionary(c => c.Type, c => c.Value)
                    };
                }

                return new TokenValidDto
                {
                    Valid = false,
                    Claims = new Dictionary<string, string>()
                };
            }
            catch (SecurityTokenException ex)
            {
                _logger.LogWarning("Token validation failed: {Message}", ex.Message);
                return new TokenValidDto
                {
                    Valid = false,
                    Claims = new Dictionary<string, string>()
                };
            }
            catch (Exception ex) when (ex is not TokenValidationException)
            {
                throw new TokenValidationException("Token validation failed", ex);
            }
        }
        #endregion

        #region Change Password
        public async Task<object> ChangePasswordAsync(ClaimsPrincipal loggedInUser, ChangePasswordDto changePasswordDto)
        {
            if (loggedInUser?.Identity?.IsAuthenticated != true)
            {
                throw new AuthenticationException("User is not authenticated");
            }

            if (changePasswordDto == null)
            {
                throw new PasswordChangeException("Password change request is null");
            }

            var user = await _userManager.GetUserAsync(loggedInUser)
                ?? throw new AuthenticationException("User not found");

            if (string.IsNullOrWhiteSpace(changePasswordDto.NewPassword))
            {
                throw new PasswordChangeException("New password is required");
            }

            if (string.IsNullOrWhiteSpace(changePasswordDto.CurrentPassword))
            {
                throw new PasswordChangeException("Current password is required");
            }

            if (changePasswordDto.NewPassword == changePasswordDto.CurrentPassword)
            {
                throw new PasswordChangeException("New password must be different from current password");
            }

            var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new PasswordChangeException($"Password change failed: {errors}");
            }

            try
            {
                await _tokenStorageService.RevokeAllUserTokensAsync(user.Id);
                _logger.LogInformation("All tokens revoked for user {UserId} after password change", user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to revoke tokens after password change for user {UserId}", user.Id);
            }

            return new { message = "Password changed successfully" };
        }
        #endregion

        #region Forgot Password
        public async Task<object> ForgotPasswordAsync(ForgotPasswordDto forgotPasswordDto)
        {
            if (forgotPasswordDto == null)
            {
                throw new PasswordResetException("Password reset request is null");
            }

            if (string.IsNullOrWhiteSpace(forgotPasswordDto.Email))
            {
                throw new PasswordResetException("Email is required");
            }

            if (!IsValidEmail(forgotPasswordDto.Email))
            {
                _logger.LogWarning("Invalid email format attempted for password reset: {Email}", forgotPasswordDto.Email);
                return new { message = "If the email exists, a password reset link has been sent" };
            }

            var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);

            if (user == null)
            {
                _logger.LogInformation("Password reset attempted for non-existent email: {Email}", forgotPasswordDto.Email);
                return new { message = "If the email exists, a password reset link has been sent" };
            }

            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                _logger.LogInformation("Password reset token generated for user {UserId}", user.Id);

                return new
                {
                    message = "If the email exists, a password reset link has been sent",
                    resetToken = token,
                    userId = user.Id
                };
            }
            catch (Exception ex)
            {
                throw new PasswordResetException("Failed to generate password reset token", ex);
            }
        }
        #endregion

        #region Reset Password
        public async Task<object> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            if (resetPasswordDto == null)
            {
                throw new PasswordResetException("Password reset request is null");
            }

            if (string.IsNullOrWhiteSpace(resetPasswordDto.UserId))
            {
                throw new PasswordResetException("User ID is required");
            }

            if (string.IsNullOrWhiteSpace(resetPasswordDto.ResetToken))
            {
                throw new PasswordResetException("Reset token is required");
            }

            if (string.IsNullOrWhiteSpace(resetPasswordDto.NewPassword))
            {
                throw new PasswordResetException("New password is required");
            }

            if (resetPasswordDto.NewPassword.Length < 6)
            {
                throw new PasswordResetException("Password must be at least 6 characters long");
            }

            var user = await _userManager.FindByIdAsync(resetPasswordDto.UserId) ?? throw new InvalidResetTokenException();
            try
            {
                var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.ResetToken, resetPasswordDto.NewPassword);

                if (!result.Succeeded)
                {
                    if (result.Errors.Any(e => e.Code.Contains("InvalidToken")))
                    {
                        throw new InvalidResetTokenException();
                    }

                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new PasswordResetException($"Password reset failed: {errors}");
                }

                try
                {
                    await _tokenStorageService.RevokeAllUserTokensAsync(user.Id);
                    _logger.LogInformation("All tokens revoked for user {UserId} after password reset", user.Id);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to revoke tokens after password reset for user {UserId}", user.Id);
                }

                return new { message = "Password reset successfully" };
            }
            catch (InvalidResetTokenException)
            {
                throw;
            }
            catch (Exception ex) when (ex is not PasswordResetException)
            {
                throw new PasswordResetException("Failed to reset password", ex);
            }
        }
        #endregion

        #region Logout User
        public async Task<object> LogoutUserAsync(ClaimsPrincipal loggedInUser)
        {
            if (loggedInUser?.Identity?.IsAuthenticated != true)
            {
                throw new AuthenticationException("User is not authenticated");
            }

            try
            {
                var token = ExtractTokenFromHeader();
                if (string.IsNullOrEmpty(token))
                {
                    throw new LogoutException("Unable to extract token from request");
                }

                DateTime expiry;
                try
                {
                    expiry = _tokenProvider.GetTokenExpiration(token);
                }
                catch (SecurityTokenException ex)
                {
                    throw new LogoutException("Invalid token format", ex);
                }

                var userId = _tokenProvider.GetUserIdFromToken(token);
                await _tokenStorageService.RevokeTokenAsync(userId, token);

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("User {UserId} logged out successfully, token revoked", userId);
                }

                return new { message = "Logged out successfully" };
            }
            catch (LogoutException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new LogoutException("Failed to process logout", ex);
            }
        }
        #endregion

        #region Helpers
        private string? ExtractTokenFromHeader()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var authHeader = httpContext?.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;

            return authHeader.Substring("Bearer ".Length).Trim();
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public async Task ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId) ?? throw new Exception("User not found");
            var decodedToken = Uri.UnescapeDataString(token);

            try
            {
                await _userManager.ConfirmEmailAsync(user, decodedToken);
            }
            catch (Exception ex)
            {
                throw new Exception("Email confirmation failed", ex);
            }
        }


        #endregion
    }
}