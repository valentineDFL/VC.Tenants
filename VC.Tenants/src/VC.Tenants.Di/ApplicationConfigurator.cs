using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VC.Tenants.Application;
using VC.Tenants.Application.Contracts;
using VC.Tenants.Application.TenantsUseCases;
using VC.Tenants.Application.TenantsUseCases.Implementations;
using VC.Tenants.Application.TenantsUseCases.Interfaces;
using VC.Tenants.Infrastructure.Implementations;

namespace VC.Tenants.Di;

internal static class ApplicationConfigurator
{
    public static IServiceCollection ConfigureTenantsApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ISlugGenerator, ByNameSlugGenerator>();
        services.AddScoped<IEmailVerifyCodeGenerator, ByDateCodeGenerator>();
        services.AddSingleton<ITenantEmailVerificationMessagesFactory, TenantEmailVerifyMessagesFactory>();

        services.AddScoped<IGetTenantUseCase, GetTenantUseCase>();
        services.AddScoped<IGetTenantIdByUserIdUseCase, GetTenantIdByUserIdUseCase>();
        services.AddScoped<ICreateTenantUseCase, CreateTenantUseCase>();
        services.AddScoped<IVerifyTenantEmailUseCase, VerifyTenantUseCase>();
        services.AddScoped<ISendVerificationMailUseCase, SendVerificationMailUseCase>();
        services.AddScoped<IUpdateTenantUseCase, UpdateTenantUseCase>();
        services.AddScoped<IDeleteTenantUseCase, DeleteTenantUseCase>();

        return services;
    }
}