using Microsoft.Extensions.DependencyInjection;
using VC.Tenants.Utilities.Resolvers;
using Microsoft.Extensions.Configuration;

namespace VC.Tenants.Utilities;

public static class UtilitiesConfigurator
{
    public static void ConfigureUtilities(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITenantResolver, HttpContextTenantResolver>();
    }
}