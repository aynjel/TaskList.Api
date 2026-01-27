namespace TaskList.Application.DTOs.Auth;

public class AuthResponse
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty; // Access token (JWT)
    public string RefreshToken { get; set; } = string.Empty; // Refresh token for cookie
}

