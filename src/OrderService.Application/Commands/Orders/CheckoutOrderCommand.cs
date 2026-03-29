namespace OrderService.Application.Commands.Orders;

public sealed record CheckoutOrderCommand(
    Guid OrderId,
    string PaymentServiceId,
    string PaymentMethod,
    string IdempotentId);
