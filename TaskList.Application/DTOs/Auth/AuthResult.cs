namespace TaskList.Application.DTOs.Auth;

/// <summary>
/// Internal authentication result used between service and controller layers.
/// Contains refresh token for setting in HTTP-only cookie.
/// This DTO is NOT returned to clients.
/// </summary>
internal class AuthResult
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
}
