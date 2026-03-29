using OrderService.Domain.Entities;

namespace OrderService.Application.Commands.Orders;

public sealed record CreateOrderCommand(
    DateTime Date,
    decimal TotalAmount,
    decimal TotalDiscountAmount,
    decimal TotalPaymentAmount,
    IReadOnlyList<OrderLineSpec> Items,
    string IdempotentId);
