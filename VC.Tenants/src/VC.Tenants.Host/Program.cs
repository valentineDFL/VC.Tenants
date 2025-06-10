using Scalar.AspNetCore;
using VC.Tenants.Api;
using VC.Tenants.Di;
using VC.Shared.Utilities;
using VC.Tenants.Host;
using Serilog;
using Mapster;
using VC.Tenants.Host.Background_Services;
using VC.Shared.Integrations.Di;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddApplicationPart(typeof(Entry).Assembly);

builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

builder.Services.ConfigureIntegrationsModule(builder.Configuration);
builder.Services.ConfigureTenantsModule(builder.Configuration);
builder.Services.ConfigureUtilities(builder.Configuration);
builder.Services.ConfigureHost();

builder.Services.AddHostedService<OutboxBackgroundService>();

builder.Services.AddHttpLogging();
builder.Services.AddHealthChecks();

builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration));

builder.Services.AddMapster();

var app = builder.Build();
await EFCoreAutoMigrator.ApplyUnAplliedMigrationsAsync(app);

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