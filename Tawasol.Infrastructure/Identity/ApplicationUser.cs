using System;
using Microsoft.AspNetCore.Identity;

namespace Tawasol.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        // You can add infrastructure-specific properties here if needed.
        // For example, properties that are not part of the core domain
        // but are required for authentication or external integrations.
    }
}
