using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;

namespace OrderService.Infrastructure.Persistence.Configurations;

public sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Payload).IsRequired();
        builder.Property(m => m.Status).HasConversion<int>();
        builder.Property(m => m.CreatedAtUtc).IsRequired();

        builder.HasIndex(m => new { m.EntityId, m.Status });
        builder.HasIndex(m => m.CreatedAtUtc);
    }
}
