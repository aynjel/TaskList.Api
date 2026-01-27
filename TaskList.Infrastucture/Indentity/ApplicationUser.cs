using Microsoft.AspNetCore.Identity;

namespace TaskList.Infrastucture.Indentity;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
}
