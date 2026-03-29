using OrderService.Domain.Shared.Enums;

namespace OrderService.Application.ReadModels;

public sealed record OrderSummaryReadModel(
    Guid Id,
    string Code,
    DateTime OrderDate,
    OrderStatus Status,
    PaymentStatus PaymentStatus,
    DateTime CreatedAtUtc,
    Guid CreatedBy,
    string CreatedByName,
    decimal TotalAmount,
    decimal TotalDiscountAmount,
    decimal TotalPaymentAmount);

public sealed record PagedOrdersReadModel(
    int TotalItems,
    int TotalPages,
    int Page,
    int Size,
    IReadOnlyList<OrderSummaryReadModel> Items);
