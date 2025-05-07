using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VC.Tenants.Entities;

namespace VC.Tenants.Infrastructure.Persistence.EntityConfigurations;

internal class TenantRelationConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired();

        builder.Property(t => t.Slug)
            .IsRequired();

        builder.OwnsOne(t => t.Config);
        
        builder.OwnsOne(t => t.ContactInfo, ci =>
        {
            ci.OwnsOne(add => add.Address);
            ci.OwnsOne(ema => ema.EmailAddress);
        });

        builder.OwnsOne(t => t.WorkSchedule, t =>
        {
            t.OwnsMany(t => t.WeekSchedule, ws =>
             {
                ws.WithOwner().HasForeignKey(x => x.TenantId);
                ws.Property(x => x.Id);
                ws.HasKey(x => x.Id);
            });
        });
    }
}