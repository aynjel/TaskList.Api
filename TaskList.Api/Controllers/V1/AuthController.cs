using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Api.Controllers.Common;
using TaskList.Application.DTOs.Auth;
using TaskList.Application.Interfaces;

namespace TaskList.Api.Controllers.V1;

[ApiVersion("1.0")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger, IConfiguration configuration) 
    : BaseApiController(logger, configuration)
{
    private readonly IAuthService _authService = authService;

    /// <summary>
    /// Register a new user
    /// </summary>
    /// <param name="request">Registration request containing name, email, and password</param>
    /// <returns>User information with JWT access token</returns>
    [HttpPost("register")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);
            SetRefreshTokenCookie(response.RefreshToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return HandleInvalidOperation<AuthResponse>(ex);
        }
        catch (Exception ex)
        {
            return HandleException<AuthResponse>(ex, "An error occurred during registration.");
        }
    }

    /// <summary>
    /// Login with email and password
    /// </summary>
    /// <param name="request">Login request containing email and password</param>
    /// <returns>User information with JWT access token</returns>
    [HttpPost("login")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            SetRefreshTokenCookie(response.RefreshToken);
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return HandleInvalidOperation<AuthResponse>(ex);
        }
        catch (Exception ex)
        {
            return HandleException<AuthResponse>(ex, "An error occurred during login.");
        }
    }

    /// <summary>
    /// Refresh access token using refresh token from cookie
    /// </summary>
    /// <returns>New JWT access token</returns>
    [HttpPost("refresh")]
    [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Refresh()
    {
        try
        {
            var refreshToken = GetRefreshTokenFromCookie();
            
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized<AuthResponse>("Refresh token is required.");
            }

            var response = await _authService.RefreshTokenAsync(refreshToken);
            SetRefreshTokenCookie(response.RefreshToken);
            
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized<AuthResponse>(ex.Message);
        }
        catch (Exception ex)
        {
            return HandleException<AuthResponse>(ex, "An error occurred during token refresh.");
        }
    }

    /// <summary>
    /// Logout and revoke refresh token
    /// </summary>
    /// <returns>Success message</returns>
    [HttpPost("logout")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        try
        {
            var refreshToken = GetRefreshTokenFromCookie();
            
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _authService.RevokeTokenAsync(refreshToken);
            }
            
            ClearRefreshTokenCookie();
            
            return Ok(new { message = "Logged out successfully." });
        }
        catch (Exception ex)
        {
            return HandleException(ex, "An error occurred during logout.");
        }
    }

    /// <summary>
    /// Get current authenticated user's information
    /// </summary>
    /// <returns>Current user details</returns>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(CurrentUserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CurrentUserResponse>> GetCurrentUser()
    {
        try
        {
            var userId = CurrentUserId;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized<CurrentUserResponse>("Invalid token.");
            }

            var currentUser = await _authService.GetCurrentUserAsync(userId);
            
            if (currentUser == null)
            {
                return NotFound<CurrentUserResponse>("User not found.");
            }

            return Ok(currentUser);
        }
        catch (Exception ex)
        {
            return HandleException<CurrentUserResponse>(ex, "An error occurred while retrieving user information.");
        }
    }
}

