using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VC.Tenants.Api.OpenApi;

namespace VC.Tenants.Di;

public static class ModuleConfigurator
{
    public static void ConfigureTenantsOpenApi(this IServiceCollection services, IConfiguration configuration)
        => services.AddOpenApi(OpenApiConfig.DocumentName, opts => OpenApiConfig.ConfigureOpenApi(opts));

    public static void ConfigureTenantsModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureTenantsApiExtensions(configuration);
        services.ConfigureTenantsApplication(configuration);
        services.ConfigureTenantsInfrastructure(configuration);
        services.Configure<TenantsModuleSettings>(configuration.GetSection(nameof(TenantsModuleSettings)));

        services.ConfigureTenantsOpenApi(configuration);
    }
}