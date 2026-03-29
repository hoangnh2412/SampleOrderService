using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;
using OrderService.Domain.Shared.Constants;

namespace OrderService.Infrastructure.Persistence.Configurations;

public sealed class OrderMessageConfiguration : IEntityTypeConfiguration<OrderMessage>
{
    public void Configure(EntityTypeBuilder<OrderMessage> builder)
    {
        builder.ToTable("OrderMessages");
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Payload).IsRequired().HasMaxLength(OrderConstraints.OrderMessagePayloadMaxLength);
        builder.Property(m => m.Status).HasConversion<int>();
        builder.Property(m => m.CreatedAtUtc).IsRequired();

        builder.HasIndex(m => new { m.EntityId, m.Status });
    }
}
