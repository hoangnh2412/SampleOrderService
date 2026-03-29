using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.Entities;
using OrderService.Domain.Shared.Constants;

namespace OrderService.Infrastructure.Persistence.Configurations;

public sealed class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
{
    public void Configure(EntityTypeBuilder<OrderDetail> builder)
    {
        builder.ToTable("OrderDetails");
        builder.HasKey(d => d.Id);
        builder.Property(d => d.ProductName).IsRequired().HasMaxLength(OrderConstraints.ProductNameMaxLength);
        builder.Property(d => d.UnitPrice).HasPrecision(18, 2);
        builder.Property(d => d.Amount).HasPrecision(18, 2);
        builder.Property(d => d.DiscountAmount).HasPrecision(18, 2);
        builder.Property(d => d.PaymentAmount).HasPrecision(18, 2);
        builder.Property(d => d.ProductId).IsRequired().HasMaxLength(OrderConstraints.ProductIdMaxLength);
    }
}
