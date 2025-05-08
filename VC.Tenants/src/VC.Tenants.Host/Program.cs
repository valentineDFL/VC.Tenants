using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using VC.Tenants.Api;
using VC.Tenants.Di;
using VC.Shared.Utilities;
using VC.Tenants.Host;
using Serilog;
using Mapster;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddApplicationPart(typeof(Entry).Assembly);

builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureTenantsModule(builder.Configuration);
builder.Services.ConfigureUtilities(builder.Configuration);
builder.Services.ConfigureHost();

builder.Services.AddHttpLogging();
builder.Services.AddHealthChecks();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddMapster();

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