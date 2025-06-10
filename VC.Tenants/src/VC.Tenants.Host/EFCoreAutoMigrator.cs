using Microsoft.EntityFrameworkCore;

namespace VC.Tenants.Host;

internal class EFCoreAutoMigrator
{
    public static async Task ApplyUnAplliedMigrationsAsync(WebApplication app)
    {
        var scope = app.Services.CreateScope();

        var dbContextsTypes = AppDomain.CurrentDomain
            .GetAssemblies()
            .Where(asm => asm.FullName.Contains("Infrastructure"))
            .SelectMany(asm => asm.GetTypes())
            .Where(t => t.IsSubclassOf(typeof(DbContext)));

        await Parallel.ForEachAsync(dbContextsTypes, async (dbContextType, task) =>
        {
            var dbContextInstance = scope.ServiceProvider.GetRequiredService(dbContextType);

            if (dbContextInstance is not DbContext dbContext)
                return;

            var pendingMigrations = dbContext.Database.GetPendingMigrations();

            if (pendingMigrations.Any())
                await dbContext.Database.MigrateAsync();
        });
    }
}