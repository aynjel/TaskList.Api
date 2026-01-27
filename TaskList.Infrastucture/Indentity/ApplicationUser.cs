using Microsoft.AspNetCore.Identity;

namespace TaskList.Infrastucture.Indentity;

public class ApplicationUser : IdentityUser
{
    public string Name { get; set; } = string.Empty;
}
