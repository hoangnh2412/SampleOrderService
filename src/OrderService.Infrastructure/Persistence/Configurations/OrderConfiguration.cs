using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;
using OrderService.Domain.Shared.Constants;

namespace OrderService.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Code).IsRequired().HasMaxLength(OrderConstraints.CodeMaxLength);
        builder.Property(o => o.OrderDate).IsRequired();
        builder.Property(o => o.TotalAmount).HasPrecision(18, 2);
        builder.Property(o => o.TotalDiscountAmount).HasPrecision(18, 2);
        builder.Property(o => o.TotalPaymentAmount).HasPrecision(18, 2);
        builder.Property(o => o.Status).HasConversion<int>();
        builder.Property(o => o.PaymentStatus).HasConversion<int>();
        builder.Property(o => o.CreatedAtUtc).IsRequired();
        builder.Property(o => o.CreatedBy).IsRequired();
        builder.Property(o => o.CreatedByName).IsRequired().HasMaxLength(OrderConstraints.PersonNameMaxLength);
        builder.Property(o => o.PaymentByName).HasMaxLength(OrderConstraints.PersonNameMaxLength);
        builder.Property(o => o.IdempotentId).IsRequired().HasMaxLength(OrderConstraints.IdempotentIdMaxLength);

        builder.HasIndex(o => o.IdempotentId)
            .IsUnique()
            .HasFilter("IdempotentId <> ''");

        builder.HasMany(o => o.Details)
            .WithOne()
            .HasForeignKey(d => d.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
