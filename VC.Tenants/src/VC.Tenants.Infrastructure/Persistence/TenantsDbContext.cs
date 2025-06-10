using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using VC.Tenants.Entities;
using VC.Shared.Utilities.Resolvers;
using VC.Tenants.Entities.Tenants;
using VC.Tenants.Entities.Tenants.Schedule;
using VC.Tenants.Entities.Tenants.ContactInfos;

namespace VC.Tenants.Infrastructure.Persistence;

public class TenantsDbContext : DbContext
{
    public const string SchemaName = "tenants";
    public const string CacheKeyPrefix = "local";

    private readonly ITenantResolver _tenantResolver;
    private readonly TenantsModuleSettings _tenantModuleSettings;

    public TenantsDbContext(
        DbContextOptions<TenantsDbContext> options, 
        ITenantResolver tenantResolver,
        IOptions<TenantsModuleSettings> tenantModuleSettings) : base(options)
    {
        _tenantResolver = tenantResolver;
        _tenantModuleSettings = tenantModuleSettings.Value;
    }

    public DbSet<Tenant> Tenants { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(SchemaName);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TenantsDbContext).Assembly);

        modelBuilder.Entity<Tenant>()
            .HasQueryFilter(t => t.Id == _tenantResolver.Resolve());
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseAsyncSeeding(async (context, flag, cts) =>
        {
            if (!_tenantModuleSettings.SeedingSettings.IsEnabled)
                return;

            var isTenantExists = context.Set<Tenant>()
                .IgnoreQueryFilters()
                .Any(t => t.Slug == SeedingDataBaseKeys.SeedTenantSlug);

            if (isTenantExists)
                return;

            Tenant tenant = CreateSeedingTenant();

            await context.Set<Tenant>().AddAsync(tenant);

            await context.SaveChangesAsync();
        });
    }

    private Tenant CreateSeedingTenant()
    {
        var tenantId = Guid.Empty;

        var config = TenantConfiguration.Create
            (
                "testtesttestdwihdwd",
                "USD",
                "RU",
                "UTC"
            );

        var address = Address.Create
        (
            "Ukraine",
            "Kiev",
            "Pushkina Street",
            456
        );

        var emailAddress = EmailAddress.Create("v.clients.company@gmail.com", true);

        var contactInfo = ContactInfo.Create
        (
            "+123456789",
            address,
            emailAddress
        );

        var weekSchedule = new List<DaySchedule>
        {
            DaySchedule.Create
            (
                Guid.CreateVersion7(),
                tenantId,
                DayOfWeek.Sunday,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(8)
            ),
            DaySchedule.Create
            (
                Guid.CreateVersion7(),
                tenantId,
                DayOfWeek.Monday,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(8)
            ),
            DaySchedule.Create
            (
                Guid.CreateVersion7(),
                tenantId,
                DayOfWeek.Tuesday,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(8)
            ),
            DaySchedule.Create
            (
                Guid.CreateVersion7(),
                tenantId,
                DayOfWeek.Wednesday,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(8)
            ),
            DaySchedule.Create
            (
                Guid.CreateVersion7(),
                tenantId,
                DayOfWeek.Thursday,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(8)
            ),
            DaySchedule.Create
            (
                Guid.CreateVersion7(),
                tenantId,
                DayOfWeek.Friday,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(8)
            ),
            DaySchedule.Create
            (
                Guid.CreateVersion7(),
                tenantId,
                DayOfWeek.Saturday,
                DateTime.UtcNow,
                DateTime.UtcNow.AddHours(8)
            )
        };

        var workShedule = WorkSchedule.Create(weekSchedule);

        return Tenant.Create
        (
            tenantId,
            Guid.Empty,
            "AdminTestTenant",
            SeedingDataBaseKeys.SeedTenantSlug,
            config,
            TenantStatus.Active,
            contactInfo,
            workShedule
        );
    }
}