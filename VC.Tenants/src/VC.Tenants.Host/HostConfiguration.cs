using Asp.Versioning;
using OpenTelemetry.Metrics;

namespace VC.Host;

internal static class HostConfiguration
{
    public static void ConfigureHost(this IServiceCollection services)
    {
        AddOpenApi(services);
        AddOpenTelemetry(services);
        AddApiVersioning(services);
    }

    private static void AddOpenApi(IServiceCollection services)
    {
        services.AddOpenApi("home", opts =>
        {
            opts.ShouldInclude = description => description.GroupName == "Home API";
            opts.AddDocumentTransformer((document, ctx, ctl) =>
            {
                document.Info = new()
                {
                    Version = "v1",
                    Title = "Универсальная платформа для управления услугами и онлайн-бронирования с поддержкой мультитенантности",
                    Description = """
                                <p>
                                  <h3><a href="http://localhost:5056/scalar/tenants"
                                    title="click">Управление арендаторами</a></h3>
                                </p>
                                <p>
                                  <h3><a href="http://localhost:5056/scalar/bookings"
                                    title="click">Управление бронированиями</a></h3>
                                </p>
                                <p>
                                  <h3><a href="http://localhost:5056/scalar/services"
                                    title="click">Управление услугами</a></h3>
                                </p><br/>
                                <p>
                                  <a href="https://gitlab.com/tech-power-partners/vclients/vc 
                                    title="click"">GitLab - vclients</a>
                                </p>
                              """
                };

                return Task.CompletedTask;
            });
        });
    }

    private static void AddOpenTelemetry(IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .WithMetrics(b =>
            {
                b.AddMeter("VC", "Npgsql");

                b.AddProcessInstrumentation()
                 .AddRuntimeInstrumentation()
                 .AddAspNetCoreInstrumentation()
                 .AddHttpClientInstrumentation();

                b.AddPrometheusExporter();
            });
    }

    private static void AddApiVersioning(IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new(1);
            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ApiVersionReader = ApiVersionReader.Combine(
                new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("X-Api-Version"));
        })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });
    }
}