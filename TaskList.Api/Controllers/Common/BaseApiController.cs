using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using TaskList.Api.Extensions;
using TaskList.Application.Common;

namespace TaskList.Api.Controllers.Common;

/// <summary>
/// Base controller with common attributes and helper methods
/// </summary>
[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseApiController(ILogger logger, IConfiguration configuration) : ControllerBase
{
    protected readonly ILogger Logger = logger;
    protected readonly IConfiguration Configuration = configuration;

    /// <summary>
    /// Gets the current user's ID from JWT claims
    /// </summary>
    protected string? CurrentUserId => User.GetUserId();

    /// <summary>
    /// Gets the current user's email from JWT claims
    /// </summary>
    protected string? CurrentUserEmail => User.GetUserEmail();

    /// <summary>
    /// Gets the current user's name from JWT claims
    /// </summary>
    protected string? CurrentUserName => User.GetUserName();

    /// <summary>
    /// Handles exceptions and returns appropriate error responses
    /// </summary>
    protected ActionResult<T> HandleException<T>(Exception ex, string message = "An error occurred.")
    {
        Logger.LogError(ex, "An error occurred. {ErrorMessage}", message);
        return BadRequest(new { message });
    }

    /// <summary>
    /// Handles exceptions and returns IActionResult (non-generic)
    /// </summary>
    protected IActionResult HandleException(Exception ex, string message = "An error occurred.")
    {
        Logger.LogError(ex, "An error occurred. {ErrorMessage}", message);
        return BadRequest(new { message });
    }

    /// <summary>
    /// Handles InvalidOperationException and returns BadRequest
    /// </summary>
    protected ActionResult<T> HandleInvalidOperation<T>(InvalidOperationException ex)
    {
        Logger.LogWarning(ex, "Invalid operation: {ErrorMessage}", ex.Message);
        return BadRequest(new { message = ex.Message });
    }

    /// <summary>
    /// Returns Unauthorized with a custom message
    /// </summary>
    protected ActionResult<T> Unauthorized<T>(string message)
    {
        return base.Unauthorized(new { message });
    }

    /// <summary>
    /// Returns NotFound with a custom message
    /// </summary>
    protected ActionResult<T> NotFound<T>(string message)
    {
        return base.NotFound(new { message });
    }

    /// <summary>
    /// Sets refresh token in HTTP-only cookie
    /// </summary>
    protected void SetRefreshTokenCookie(string refreshToken)
    {
        var jwtSettings = Configuration.GetSection("JwtSettings").Get<JwtSettings>();

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(jwtSettings!.RefreshTokenExpirationInDays),
            Path = "/"
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        Logger.LogInformation("Refresh token set in cookie");
    }

    /// <summary>
    /// Gets refresh token from cookie
    /// </summary>
    protected string? GetRefreshTokenFromCookie()
    {
        return Request.Cookies["refreshToken"];
    }

    /// <summary>
    /// Clears refresh token cookie
    /// </summary>
    protected void ClearRefreshTokenCookie()
    {
        Response.Cookies.Delete("refreshToken");
        Logger.LogInformation("Refresh token cookie cleared");
    }
}
