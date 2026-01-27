using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TaskList.Infrastucture.Persistence;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            
            // Check if database exists and apply pending migrations
            if (context.Database.GetPendingMigrations().Any())
            {
                logger.LogInformation("Applying pending migrations...");
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully.");
            }
            else
            {
                logger.LogInformation("Database is up to date. No migrations to apply.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }
    }
}
