using OrderService.Domain.Shared.Enums;

namespace OrderService.Domain.Events;

/// <summary>
/// Một dòng hàng trong sự kiện <see cref="OrderCreated"/> (snapshot lúc tạo đơn).
/// </summary>
public sealed record OrderCreatedLineItem(
    Guid LineId,
    string ProductId,
    string ProductName,
    decimal UnitPrice,
    int Quantity,
    decimal Amount,
    decimal DiscountAmount,
    decimal PaymentAmount);

public sealed record OrderCreated(
    Guid OrderId,
    string Code,
    OrderStatus Status,
    PaymentStatus PaymentStatus,
    DateTime CreatedAtUtc,
    DateTime OccurredAtUtc,
    IReadOnlyList<OrderCreatedLineItem> Items) : IDomainEvent;
