using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VC.Tenants.Infrastructure;
using VC.Tenants.Infrastructure.Cache;
using VC.Tenants.Infrastructure.Persistence;
using VC.Tenants.Infrastructure.Persistence.Repositories;
using VC.Tenants.Repositories;
using VC.Tenants.UnitOfWork;
using VC.Shared.Utilities;
using VC.Tenants.Application;
using VC.Tenants.Infrastructure.Implementations.Rabbit;
using VC.Tenants.Application.Contracts;

namespace VC.Tenants.Di;

internal static class InfrastructureConfigurator
{
    public static void ConfigureTenantsInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetSection(nameof(ConnectionStrings)).Get<ConnectionStrings>();

        services.AddDbContext<TenantsDbContext>(options => options
            .UseNpgsql(connectionString.PostgresSQL, x => x.MigrationsHistoryTable("__EFMigrationsHistory", TenantsDbContext.SchemaName)));

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = connectionString.Redis;
            options.InstanceName = TenantsDbContext.CacheKeyPrefix;
        });

        ConfigureRepositories(services);
    }

    private static void ConfigureRepositories(IServiceCollection services)
    {
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IEmailVerificationRepository, EmailVerificationRedisRepository>();
        services.AddScoped<IUnitOfWork, TenantsUnitOfWork>();
        services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddScoped<IMessageHandler, RabbitMqMessageHandler>();

        services.AddSingleton<IPublisher, DirectRabbitPublisher>();
    }
}