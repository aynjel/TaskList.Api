using System.Security.Claims;

namespace TaskList.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the user ID from the ClaimsPrincipal
    /// </summary>
    /// <param name="principal">The ClaimsPrincipal (User)</param>
    /// <returns>User ID as string, or null if not found</returns>
    public static string? GetUserId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    /// <summary>
    /// Gets the user's email from the ClaimsPrincipal
    /// </summary>
    /// <param name="principal">The ClaimsPrincipal (User)</param>
    /// <returns>User email as string, or null if not found</returns>
    public static string? GetUserEmail(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Email)?.Value;
    }

    /// <summary>
    /// Gets the user's name from the ClaimsPrincipal
    /// </summary>
    /// <param name="principal">The ClaimsPrincipal (User)</param>
    /// <returns>User name as string, or null if not found</returns>
    public static string? GetUserName(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimTypes.Name)?.Value;
    }
}
