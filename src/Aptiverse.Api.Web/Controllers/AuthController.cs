using Aptiverse.Api.Web.Controllers;
using Aptiverse.Application.Auth.Dtos;
using Aptiverse.Application.Auth.Services;
using Aptiverse.Core.Dtos;
using Aptiverse.Core.Exceptions;
using Aptiverse.Domain.Models.Users;
using Aptiverse.Infrastructure.Data;
using Aptiverse.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public class AuthController(
    IAuthService authService,
    ILogger<AuthController> logger,
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IConfiguration config,
    ApplicationDbContext dbContext,
    ITokenProvider tokenProvider) : ControllerBase
{
    private readonly IAuthService _authService = authService;
    private readonly ILogger<AuthController> _logger = logger;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        try
        {
            var result = await _authService.RegisterUserAsync(registerDto);
            return Ok(result);
        }
        catch (UserRegistrationException ex)
        {
            return BadRequest(new
            {
                message = "User registration failed",
                error = ex.Message,
                type = "RegistrationError"
            });
        }
        catch (RoleAssignmentException ex)
        {
            return BadRequest(new
            {
                message = "Role assignment failed",
                error = ex.Message,
                type = "RoleError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during user registration");
            return StatusCode(500, new
            {
                message = "An unexpected error occurred during registration",
                type = "ServerError"
            });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        try
        {
            var result = await _authService.LoginUserAsync(loginDto);
            return Ok(result);
        }
        catch (AuthenticationException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message,
                type = "AuthenticationError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during user login");
            return StatusCode(500, new
            {
                message = "An unexpected error occurred during login",
                type = "ServerError"
            });
        }
    }

    [HttpPost("refresh-token")]
    [Authorize]
    public async Task<IActionResult> RefreshToken()
    {
        try
        {
            var result = await _authService.RefreshTokenAsync(User);
            return Ok(result);
        }
        catch (TokenRefreshException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message,
                type = "TokenRefreshError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during token refresh");
            return StatusCode(500, new
            {
                message = "An unexpected error occurred during token refresh",
                type = "ServerError"
            });
        }
    }

    [HttpPost("validate-token")]
    public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenDto validateTokenDto)
    {
        try
        {
            var result = await _authService.ValidateTokenAsync(validateTokenDto);
            return Ok(result);
        }
        catch (TokenValidationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message,
                type = "TokenValidationError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during token validation");
            return StatusCode(500, new
            {
                message = "An unexpected error occurred during token validation",
                type = "ServerError"
            });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var result = await _authService.LogoutUserAsync(User);
            return Ok(result);
        }
        catch (AuthenticationException ex)
        {
            // Still return success for better UX
            return Ok(new { message = "Logged out successfully" });
        }
        catch (LogoutException ex)
        {
            _logger.LogError(ex, "Error during logout process");
            // Still return success for better UX
            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during logout");
            // Still return success for better UX
            return Ok(new { message = "Logged out successfully" });
        }
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
    {
        try
        {
            var result = await _authService.ChangePasswordAsync(User, changePasswordDto);
            return Ok(result);
        }
        catch (AuthenticationException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message,
                type = "AuthenticationError"
            });
        }
        catch (PasswordChangeException ex)
        {
            return BadRequest(new
            {
                message = ex.Message,
                type = "PasswordChangeError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during password change");
            return StatusCode(500, new
            {
                message = "An unexpected error occurred while changing password",
                type = "ServerError"
            });
        }
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
    {
        try
        {
            var result = await _authService.ForgotPasswordAsync(forgotPasswordDto);
            return Ok(result);
        }
        catch (PasswordResetException ex)
        {
            return BadRequest(new
            {
                message = ex.Message,
                type = "PasswordResetError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during password reset request");
            return StatusCode(500, new
            {
                message = "An unexpected error occurred while processing your request",
                type = "ServerError"
            });
        }
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
    {
        try
        {
            var result = await _authService.ResetPasswordAsync(resetPasswordDto);
            return Ok(result);
        }
        catch (InvalidResetTokenException ex)
        {
            return BadRequest(new
            {
                message = ex.Message,
                type = "InvalidResetToken"
            });
        }
        catch (PasswordResetException ex)
        {
            return BadRequest(new
            {
                message = ex.Message,
                type = "PasswordResetError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during password reset");
            return StatusCode(500, new
            {
                message = "An unexpected error occurred while resetting password",
                type = "ServerError"
            });
        }
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        try
        {
            var result = await _authService.GetCurrentUserAsync(User);
            return Ok(result);
        }
        catch (AuthenticationException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message,
                type = "AuthenticationError"
            });
        }
        catch (UserRetrievalException ex)
        {
            return Unauthorized(new
            {
                message = ex.Message,
                type = "UserRetrievalError"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while retrieving current user");
            return StatusCode(500, new
            {
                message = "An unexpected error occurred while retrieving user information",
                type = "ServerError"
            });
        }
    }
}