using OrderService.Application.Abstractions;
using OrderService.Application.Commands.Orders;
using OrderService.Application.Dtos.Orders;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces;
using OrderService.Domain.Repositories;

namespace OrderService.Application.Features.Orders.Checkout;

public sealed class CheckoutOrderHandler(
    IOrderRepository orderRepo,
    TimeProvider time,
    IUserContext user,
    IPaymentGatewayClient paymentGateway)
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

        var order = await orderRepo.LoadByIdAsync(command.OrderId, cancellationToken)
            ?? throw new ApiNotFoundException(ErrorCodes.OrderNotFound, "Order not found.");

        if (order.PaymentStatus == PaymentStatus.Paid)
        {
            if (string.IsNullOrWhiteSpace(command.IdempotentId))
                throw new ApiConflictException(ErrorCodes.OrderAlreadyPaid, "Order has already been paid.");

            var paymentHistory = await orderRepo.GetPaymentHistoryByEntityIdAsync(order.Id, cancellationToken)
                ?? throw new ApiNotFoundException(ErrorCodes.PaymentHistoryNotFound, "Payment history not found.");

            return new CheckoutResultDto
            {
                OrderId = order.Id,
                Status = order.Status.ToString(),
                PaymentTransactionId = paymentHistory.TransactionId,
                PaymentAt = order.PaymentAt,
                PaymentBy = user.Id.ToString(),
                PaymentByName = paymentHistory.CreatedByName!
            };
        }

        await paymentGateway.PrePaymentAsync(order.Id, cancellationToken);
        order.ProcessPayment();
        await orderRepo.SaveAsync(order, cancellationToken);

        return new CheckoutResultDto
        {
            OrderId = order.Id,
            Status = order.Status.ToString(),
        };
    }
}
