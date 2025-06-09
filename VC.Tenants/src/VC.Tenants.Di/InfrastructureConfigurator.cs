using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VC.Tenants.Infrastructure;
using VC.Tenants.Infrastructure.Cache;
using VC.Tenants.Infrastructure.Persistence;
using VC.Tenants.Infrastructure.Persistence.Repositories;
using VC.Tenants.Repositories;
using VC.Tenants.UnitOfWork;
using VC.Tenants.Application;
using VC.Shared.MailkitIntegration;
using VC.Tenants.Infrastructure.Implementations;
using VC.Shared.Utilities.Options;

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

        ConfigureRepositories(services, configuration);
    }

    private static void ConfigureRepositories(IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IEmailVerificationRepository, EmailVerificationRedisRepository>();
        services.AddScoped<IUnitOfWork, PostgresTenantsUnitOfWork>();
        services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddScoped<IOutboxMessageHandler<Message>, MailSendMessageHandler>();

        services.Configure<ConnectionStrings>(configuration.GetSection(nameof(ConnectionStrings)));
    }
}