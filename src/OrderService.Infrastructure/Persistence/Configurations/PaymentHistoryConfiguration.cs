using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;
using OrderService.Domain.Shared.Constants;

namespace OrderService.Infrastructure.Persistence.Configurations;

public sealed class PaymentHistoryConfiguration : IEntityTypeConfiguration<PaymentHistory>
{
    public void Configure(EntityTypeBuilder<PaymentHistory> builder)
    {
        builder.ToTable("PaymentHistories");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.EntityType).HasConversion<int>();
        builder.Property(p => p.Amount).HasPrecision(18, 2);
        builder.Property(p => p.TransactionId).IsRequired().HasMaxLength(OrderConstraints.TransactionIdMaxLength);
        builder.Property(p => p.CreatedAtUtc).IsRequired();
        builder.Property(p => p.CreatedByName).HasMaxLength(OrderConstraints.PersonNameMaxLength);

        builder.HasIndex(p => new { p.EntityId, p.EntityType });
    }
}
