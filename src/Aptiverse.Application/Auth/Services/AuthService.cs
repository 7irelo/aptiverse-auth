using Aptiverse.Application.Auth.Dtos;
using Aptiverse.Application.Users.Dtos;
using Aptiverse.Core.Dtos;
using Aptiverse.Core.Exceptions;
using Aptiverse.Domain.Models.Users;
using Aptiverse.Infrastructure.Data;
using Aptiverse.Infrastructure.Services;
using Aptiverse.Infrastructure.Utilities;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Aptiverse.Application.Auth.Services
{
    public class AuthService(
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext dbContext,
        IMapper mapper,
        ITokenProvider tokenProvider,
        IConfiguration config,
        ITokenStorageService tokenStorageService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthService> logger,
        IEmailSender emailSender,
        SignInManager<ApplicationUser> signInManager) : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IMapper _mapper = mapper;
        private readonly ITokenProvider _tokenProvider = tokenProvider;
        private readonly IConfiguration _config = config;
        private readonly ITokenStorageService _tokenStorageService = tokenStorageService;
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ILogger<AuthService> _logger = logger;
        private readonly IEmailSender _emailSender = emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;

        public async Task<UserDto> GetCurrentUserAsync(ClaimsPrincipal loggedInUser)
        {
            if (loggedInUser?.Identity?.IsAuthenticated != true)
            {
                _logger.LogWarning("GetCurrentUserAsync: User is not authenticated");
                throw new AuthenticationException("User is not authenticated");
            }

            try
            {
                var token = ExtractTokenFromHeader();
                if (string.IsNullOrEmpty(token))
                {
                    _logger.LogWarning("GetCurrentUserAsync: No token provided in header");
                    throw new AuthenticationException("No token provided");
                }

                _logger.LogInformation("GetCurrentUserAsync: Extracted token, validating...");

                var validatedPrincipal = await _tokenProvider.ValidateTokenAsync(token);
                if (validatedPrincipal == null)
                {
                    _logger.LogWarning("GetCurrentUserAsync: Token validation returned null");
                    throw new AuthenticationException("Invalid or expired token");
                }

                var userId = validatedPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
                _logger.LogInformation("GetCurrentUserAsync: Token validated for user {UserId}", userId);

                var isTokenValid = await _tokenStorageService.IsTokenValidAsync(userId, token);
                if (!isTokenValid)
                {
                    _logger.LogWarning("GetCurrentUserAsync: Token not found in storage for user {UserId}", userId);
                    throw new AuthenticationException("Token has been revoked or not stored properly");
                }

                var user = await _userManager.GetUserAsync(validatedPrincipal);
                if (user == null)
                {
                    _logger.LogWarning("GetCurrentUserAsync: User not found for ID {UserId}", userId);
                    throw new UserRetrievalException("User account not found");
                }

                _logger.LogInformation("GetCurrentUserAsync: Successfully retrieved user {UserName}", user.UserName);

                var roles = await _userManager.GetRolesAsync(user);

                return new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Roles = [.. roles],
                    UserType = roles.FirstOrDefault() ?? "User"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCurrentUserAsync: Unexpected error");
                throw;
            }
        }

        public async Task<TokenDto<UserDto>> RegisterUserAsync(RegisterDto registerDto)
        {
            var user = _mapper.Map<ApplicationUser>(registerDto);
            user.CreatedAt = DateTime.UtcNow;

            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (!result.Succeeded)
            {
                throw new UserRegistrationException($"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            try
            {
                var roleCreationResult = await EntityRoleCreator.CreateRoleSpecificEntity(_dbContext, user.Id, registerDto.UserType);

                if (!roleCreationResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    throw new RoleAssignmentException($"Role creation failed: {roleCreationResult.Errors}");
                }

                var roleResult = await _userManager.AddToRoleAsync(user, registerDto.UserType);

                if (!roleResult.Succeeded)
                {
                    _logger.LogWarning("Could not add user to role: {Errors}",
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }

                var roles = await _userManager.GetRolesAsync(user);
                var token = await _tokenProvider.GenerateJwtTokenAsync(user, roles);

                var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                var confirmationLink = $"https://aptiverse.co.za/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(emailConfirmationToken)}";

                var htmlMessage = $@"
                    <!DOCTYPE html>
                    <html lang='en'>
                        <head>
                            <meta charset='UTF-8'>
                            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                            <title>Confirm Your Email - Aptiverse</title>
                            <style>
                                body {{
                                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                                    line-height: 1.6;
                                    color: #333;
                                    margin: 0;
                                    padding: 0;
                                    background-color: #f4f4f4;
                                }}
                                .container {{
                                    max-width: 600px;
                                    margin: 0 auto;
                                    background: #ffffff;
                                    border-radius: 10px;
                                    overflow: hidden;
                                    box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
                                }}
                                .header {{
                                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                                    color: white;
                                    padding: 30px 20px;
                                    text-align: center;
                                }}
                                .header h1 {{
                                    margin: 0;
                                    font-size: 28px;
                                    font-weight: 300;
                                }}
                                .content {{
                                    padding: 40px 30px;
                                }}
                                .welcome-text {{
                                    font-size: 18px;
                                    margin-bottom: 20px;
                                    color: #555;
                                }}
                                .confirmation-button {{
                                    display: inline-block;
                                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                                    color: white;
                                    padding: 15px 30px;
                                    text-decoration: none;
                                    border-radius: 25px;
                                    font-weight: 600;
                                    font-size: 16px;
                                    margin: 20px 0;
                                    transition: transform 0.2s, box-shadow 0.2s;
                                }}
                                .confirmation-button:hover {{
                                    transform: translateY(-2px);
                                    box-shadow: 0 6px 12px rgba(102, 126, 234, 0.3);
                                }}
                                .alternative-text {{
                                    color: #666;
                                    margin: 25px 0 15px 0;
                                    font-size: 14px;
                                }}
                                .confirmation-link {{
                                    background: #f8f9fa;
                                    border: 1px solid #e9ecef;
                                    border-radius: 5px;
                                    padding: 15px;
                                    word-break: break-all;
                                    font-family: 'Courier New', monospace;
                                    font-size: 12px;
                                    color: #495057;
                                }}
                                .footer {{
                                    background: #f8f9fa;
                                    padding: 20px;
                                    text-align: center;
                                    color: #6c757d;
                                    font-size: 12px;
                                    border-top: 1px solid #e9ecef;
                                }}
                                .support-info {{
                                    margin-top: 15px;
                                    font-size: 14px;
                                    color: #495057;
                                }}
                                .user-info {{
                                    background: #e8f4fd;
                                    border-left: 4px solid #2196F3;
                                    padding: 15px;
                                    margin: 20px 0;
                                    border-radius: 4px;
                                }}
                                .steps {{
                                    margin: 25px 0;
                                }}
                                .step {{
                                    display: flex;
                                    align-items: flex-start;
                                    margin-bottom: 15px;
                                }}
                                .step-number {{
                                background: #667eea;
                                color: white;
                                width: 24px;
                                height: 24px;
                                border-radius: 50%;
                                display: inline-block; /* Change from flex */
                                text-align: center;
                                line-height: 24px; /* Match the height */
                                font-size: 14px;
                                font-weight: bold;
                                margin-right: 15px;
                                flex-shrink: 0;
                                }}
                            </style>
                        </head>
                        <body>
                            <div class='container'>
                                <div class='header'>
                                    <h1>Welcome to Aptiverse! 🎉</h1>
                                </div>
        
                                <div class='content'>
                                    <p class='welcome-text'>Hello <strong>{user.FirstName} {user.LastName}</strong>,</p>
                                    <p>Thank you for joining Aptiverse! We're excited to have you on board. To complete your registration and start using your account, please confirm your email address.</p>
            
                                    <div class='user-info'>
                                        <strong>Account Details:</strong><br>
                                        • Username: <strong>{user.UserName}</strong><br>
                                        • Email: <strong>{user.Email}</strong><br>
                                        • Account Type: <strong>{registerDto.UserType}</strong>
                                    </div>

                                    <div class='steps'>
                                        <div class='step'>
                                            <div class='step-number'>1</div>
                                            <div>Click the confirmation button below</div>
                                        </div>
                                        <div class='step'>
                                            <div class='step-number'>2</div>
                                            <div>You'll be redirected to verify your email</div>
                                        </div>
                                        <div class='step'>
                                            <div class='step-number'>3</div>
                                            <div>Start exploring Aptiverse features!</div>
                                        </div>
                                    </div>

                                    <div style='text-align: center;'>
                                        <a href='{confirmationLink}' class='confirmation-button'>
                                            Confirm My Email Address
                                        </a>
                                    </div>

                                    <p class='alternative-text'>If the button doesn't work, copy and paste this URL into your browser:</p>
                                    <div class='confirmation-link'>{confirmationLink}</div>

                                    <div class='support-info'>
                                        <p><strong>Need help?</strong><br>
                                        If you didn't create this account or need assistance, please contact our support team immediately.</p>
                                        <p>This confirmation link will expire in 24 hours for security reasons.</p>
                                    </div>
                                </div>
        
                                <div class='footer'>
                                    <p>&copy; {DateTime.UtcNow.Year} Aptiverse. All rights reserved.</p>
                                    <p>This is an automated message, please do not reply to this email.</p>
                                </div>
                            </div>
                        </body>
                    </html>";

                await _emailSender.SendEmailAsync(user.Email, "Confirm Your Email - Aptiverse", htmlMessage);

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
                        UserType = registerDto.UserType,
                    },
                    Message = $"User registered successfully as {registerDto.UserType}. Please check your email to confirm your account."
                };
            }
            catch (Exception ex) when (ex is not UserRegistrationException and not RoleAssignmentException)
            {
                await _userManager.DeleteAsync(user);
                throw new UserRegistrationException("An unexpected error occurred during user registration", ex);
            }
        }

        public async Task<TokenDto<UserDto>> LoginUserAsync(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username)
                ?? throw new AuthenticationException("Invalid username or password");
            var signInResult = await _signInManager.PasswordSignInAsync(dto.Username, dto.Password, true, false);

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid)
            {
                throw new AuthenticationException("Invalid username or password");
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
                    UserType = roles.FirstOrDefault() ?? "User",
                },
                Message = "Login successful"
            };
        }

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
                        UserType = roles.FirstOrDefault() ?? "User",
                    },
                    Message = "Token Refresh Successful"
                };
            }
            catch (Exception ex) when (ex is not TokenRefreshException)
            {
                throw new TokenRefreshException("Failed to generate new token", ex);
            }
        }

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

            var user = await _userManager.FindByIdAsync(resetPasswordDto.UserId);
            if (user == null)
            {
                throw new InvalidResetTokenException();
            }

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

                _logger.LogInformation("User {UserId} logged out successfully", userId);

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

        private string ExtractTokenFromHeader()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var authHeader = httpContext?.Request.Headers["Authorization"].FirstOrDefault();

            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                return null;

            return authHeader.Substring("Bearer ".Length).Trim();
        }

        private bool IsValidEmail(string email)
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
    }
}