using System.Security.Claims;

namespace TaskList.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(string userId, string name, string email);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
}

