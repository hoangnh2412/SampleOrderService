using OrderService.Application.Abstractions;
using OrderService.Application.Commands.Orders;
using OrderService.Application.Dtos.Orders;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;

namespace OrderService.Application.Features.Orders.Checkout;

public sealed class CheckoutOrderHandler(
    IOrderRepository orders,
    TimeProvider time,
    IUserContext user)
    : ICommandHandler<CheckoutOrderCommand, CheckoutResultDto>
{
    public async Task<CheckoutResultDto> HandleAsync(CheckoutOrderCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.PaymentServiceId))
            throw new ApiBadRequestException(
                ErrorCodes.InvalidRequestBody,
                "Invalid request body.",
                [new FieldError("paymentServiceId", "Required.")]);

        var utc = time.GetUtcNow().UtcDateTime;

        var order = await orders.LoadByIdAsync(command.OrderId, cancellationToken)
            ?? throw new ApiNotFoundException(ErrorCodes.OrderNotFound, "Order not found.");

        if (order.PaymentStatus == PaymentStatus.Paid)
        {
            if (string.IsNullOrWhiteSpace(command.IdempotentId))
                throw new ApiConflictException(ErrorCodes.OrderAlreadyPaid, "Order has already been paid.");

            return MapCheckout(order, user, paymentTransactionId: null);
        }

        order.CompleteCheckout(user.Id, user.FullName ?? user.UserName, utc);

        var transactionId = $"txn_{Guid.NewGuid():N}";
        var history = PaymentHistory.CreateForOrder(
            order.Id,
            order.TotalPaymentAmount,
            transactionId,
            utc,
            user.Id,
            user.FullName ?? user.UserName);

        await orders.SaveAsync(order, cancellationToken);
        await orders.AddPaymentHistoryAsync(history, cancellationToken);

        var refreshed = await orders.GetByIdWithDetailsReadOnlyAsync(order.Id, cancellationToken) ?? order;
        return MapCheckout(refreshed, user, transactionId);
    }

    private static CheckoutResultDto MapCheckout(Order o, IUserContext user, string? paymentTransactionId)
    {
        string payerDisplay;
        if (!string.IsNullOrEmpty(o.PaymentByName))
            payerDisplay = o.PaymentByName;
        else if (!string.IsNullOrEmpty(user.FullName))
            payerDisplay = user.FullName;
        else
            payerDisplay = user.UserName;

        return new CheckoutResultDto
        {
            OrderId = o.Id,
            Status = o.Status.ToString(),
            PaymentTransactionId = paymentTransactionId,
            PaymentAt = o.PaymentAt,
            PaymentBy = user.UserName,
            PaymentByName = payerDisplay
        };
    }
}
