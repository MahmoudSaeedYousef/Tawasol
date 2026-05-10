using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Tawasol.Infrastructure.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;
        
        try
        {
            // Migrate Application Database
            var context = services.GetRequiredService<AppDbContext>();
            if (context.Database.IsSqlServer())
            {
                await context.Database.MigrateAsync();
            }

            // Migrate Identity Database
            var identityContext = services.GetRequiredService<AppIdentityDbContext>();
            if (identityContext.Database.IsSqlServer())
            {
                await identityContext.Database.MigrateAsync();
            }
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<AppDbContext>>();
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }
    }
}
