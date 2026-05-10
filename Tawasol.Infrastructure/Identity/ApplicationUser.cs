using Microsoft.AspNetCore.Identity;

namespace Tawasol.Infrastructure.Identity;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public int Points { get; set; }
}
