using System.Text.Json.Serialization;

namespace TaskList.Application.DTOs.Auth;

/// <summary>
/// Authentication response containing access token and user information.
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// User's display name
    /// </summary>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// User's email address
    /// </summary>
    public string Email { get; set; } = string.Empty;
    
    /// <summary>
    /// JWT access token for API authentication (short-lived)
    /// </summary>
    public string Token { get; set; } = string.Empty;
    
    /// <summary>
    /// Token expiration time in seconds
    /// </summary>
    public int ExpiresIn { get; set; }
    
    /// <summary>
    /// Refresh token - FOR INTERNAL USE ONLY
    /// This property is used internally to pass the refresh token from service to controller.
    /// The controller sets it in HTTP-only cookie and removes it from the response body.
    /// ⚠️ This is NOT returned to clients in the API response for security reasons.
    /// </summary>
    [JsonIgnore]
    public string RefreshToken { get; set; } = string.Empty;
}

