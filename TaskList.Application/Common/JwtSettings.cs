namespace TaskList.Application.Common;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpirationInMinutes { get; set; } = 30; // Short-lived access token
    public int RefreshTokenExpirationInDays { get; set; } = 7; // Long-lived refresh token
}

