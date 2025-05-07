using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using VC.Tenants.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddApplicationPart(typeof(Entry).Assembly);

builder.Services.AddOpenApi();

var app = builder.Build();
await ApplyUnAplliedMigrationsAsync(app);

app.MapPrometheusScrapingEndpoint();
app.MapHealthChecks("/health");
app.MapOpenApi();
app.MapScalarApiReference(opts =>
{
    opts.Theme = ScalarTheme.BluePlanet;
    opts.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
    opts.ShowSidebar = true;
});

app.UseHttpLogging();
app.MapControllers();

app.Run();

static async Task ApplyUnAplliedMigrationsAsync(WebApplication app)
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