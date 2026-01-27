using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskList.Application.Common;
using TaskList.Application.DTOs.Auth;
using TaskList.Application.Interfaces;

namespace TaskList.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger, IConfiguration configuration) : ControllerBase
{
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
            var response = await authService.RegisterAsync(request);
            
            // Set refresh token in HTTP-only cookie
            SetRefreshTokenCookie(response.Token);
            
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Registration failed for email: {Email}", request.Email);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during registration for email: {Email}", request.Email);
            return BadRequest(new { message = "An error occurred during registration." });
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
            var response = await authService.LoginAsync(request);
            
            // Set refresh token in HTTP-only cookie
            SetRefreshTokenCookie(response.Token);
            
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Login failed for email: {Email}", request.Email);
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during login for email: {Email}", request.Email);
            return BadRequest(new { message = "An error occurred during login." });
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
            var refreshToken = Request.Cookies["refreshToken"];
            
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { message = "Refresh token is required." });
            }

            var response = await authService.RefreshTokenAsync(refreshToken);
            
            // Set new refresh token in cookie
            SetRefreshTokenCookie(response.Token);
            
            return Ok(response);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Token refresh failed");
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during token refresh");
            return Unauthorized(new { message = "An error occurred during token refresh." });
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
            var refreshToken = Request.Cookies["refreshToken"];
            
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await authService.RevokeTokenAsync(refreshToken);
            }
            
            // Clear refresh token cookie
            Response.Cookies.Delete("refreshToken");
            
            logger.LogInformation("User logged out successfully");
            return Ok(new { message = "Logged out successfully." });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during logout");
            return BadRequest(new { message = "An error occurred during logout." });
        }
    }

    private void SetRefreshTokenCookie(string refreshToken)
    {
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, // Prevents JavaScript access to the cookie
            Secure = true, // Only send cookie over HTTPS
            SameSite = SameSiteMode.Strict, // CSRF protection
            Expires = DateTimeOffset.UtcNow.AddDays(jwtSettings!.RefreshTokenExpirationInDays),
            Path = "/" // Cookie available for entire application
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        
        logger.LogInformation("Refresh token set in cookie");
    }
}


