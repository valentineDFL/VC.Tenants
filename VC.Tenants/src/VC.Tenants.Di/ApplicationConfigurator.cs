using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VC.Tenants.Application;
using VC.Tenants.Application.Contracts;
using VC.Tenants.Application.Tenants;
using VC.Tenants.Infrastructure.Implementations;

namespace VC.Tenants.Di;

internal static class ApplicationConfigurator
{
    public static IServiceCollection ConfigureTenantsApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITenantsService, TenantsService>();
        services.AddScoped<ISlugGenerator, ByNameSlugGenerator>();
        services.AddScoped<IEmailVerifyCodeGenerator, ByDateCodeGenerator>();
        services.AddSingleton<ITenantEmailVerificationMessagesFactory, TenantEmailVerifyMessagesFactory>();

        return services;
    }
}