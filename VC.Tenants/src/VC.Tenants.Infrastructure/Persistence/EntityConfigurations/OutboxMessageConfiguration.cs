using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VC.Tenants.Entities;

namespace VC.Tenants.Infrastructure.Persistence.EntityConfigurations;
internal class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.HasQueryFilter(x => x.ProcessedOnUtc == null);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Type)
            .IsRequired();

        builder.Property(x => x.Content)
            .HasColumnType("jsonb")
            .IsRequired();

        builder.Property(x => x.OccuredOnUtc)
            .IsRequired();

        builder.Property(x => x.ProcessedOnUtc)
            .IsRequired(false);

        builder.Property(x => x.Error)
            .IsRequired(false);
    }
}