using OrderService.Application.Commands.Orders;
using OrderService.Application.Dtos.Orders;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;

namespace OrderService.Application.Features.Webhooks;

public sealed class PaymentWebhookHandler(
    IOrderRepository orderRepo,
    TimeProvider time)
    : ICommandHandler<PaymentWebhookCommand, CheckoutResultDto>
{
    public async Task<CheckoutResultDto> HandleAsync(PaymentWebhookCommand command, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(command.TransactionId))
            throw new ApiBadRequestException(
                ErrorCodes.InvalidRequestBody,
                "Invalid request body.",
                [new FieldError("transactionId", "Required.")]);

        if (string.IsNullOrWhiteSpace(command.Status))
            throw new ApiBadRequestException(
                ErrorCodes.InvalidRequestBody,
                "Invalid request body.",
                [new FieldError("status", "Required.")]);

        var order = await orderRepo.LoadByIdAsync(command.OrderId, cancellationToken)
            ?? throw new ApiNotFoundException(ErrorCodes.OrderNotFound, "Order not found.");

        var status = command.Status.Trim();
        if (IsFailureStatus(status))
            return await HandleFailureAsync(order, cancellationToken);

        if (!IsSuccessStatus(status))
            throw new ApiBadRequestException(
                ErrorCodes.InvalidRequestBody,
                "Invalid payment status.",
                [new FieldError("status", "Must be a recognized success or failure value.")]);

        return await HandleSuccessAsync(order, command.TransactionId.Trim(), cancellationToken);
    }

    private async Task<CheckoutResultDto> HandleSuccessAsync(
        Order order,
        string transactionId,
        CancellationToken cancellationToken)
    {
        var utc = time.GetUtcNow().UtcDateTime;

        if (order.PaymentStatus == PaymentStatus.Paid)
        {
            var byTxn = await orderRepo.GetPaymentHistoryByTransactionIdAsync(transactionId, cancellationToken);
            if (byTxn is not null && byTxn.EntityId == order.Id)
                return MapPaid(order, byTxn);

            var latest = await orderRepo.GetPaymentHistoryByEntityIdAsync(order.Id, cancellationToken);
            if (latest is not null && string.Equals(latest.TransactionId, transactionId, StringComparison.Ordinal))
                return MapPaid(order, latest);

            throw new ApiConflictException(
                ErrorCodes.PaymentWebhookTransactionMismatch,
                "Order is already paid with a different transaction.");
        }

        if (order.PaymentStatus != PaymentStatus.Pending)
        {
            throw new ApiUnprocessableEntityException(
                ErrorCodes.OrderPaymentNotPending,
                "Order is not waiting for payment confirmation.");
        }

        order.CompletePayment(order.CreatedBy, order.CreatedByName, utc);
        var history = PaymentHistory.Create(
            order.Id,
            order.TotalPaymentAmount,
            transactionId,
            utc,
            order.CreatedBy,
            order.CreatedByName);

        await orderRepo.SaveAsync(order, cancellationToken);
        await orderRepo.AddPaymentHistoryAsync(history, cancellationToken);

        return MapPaid(order, history);
    }

    private async Task<CheckoutResultDto> HandleFailureAsync(Order order, CancellationToken cancellationToken)
    {
        if (order.PaymentStatus == PaymentStatus.Paid)
        {
            var h = await orderRepo.GetPaymentHistoryByEntityIdAsync(order.Id, cancellationToken)
                ?? throw new ApiNotFoundException(ErrorCodes.PaymentHistoryNotFound, "Payment history not found.");
            return MapPaid(order, h);
        }

        if (order.PaymentStatus == PaymentStatus.Failed)
        {
            return new CheckoutResultDto
            {
                OrderId = order.Id,
                Status = order.PaymentStatus.ToString(),
                PaymentBy = string.Empty,
                PaymentByName = string.Empty
            };
        }

        if (order.PaymentStatus != PaymentStatus.Pending)
        {
            throw new ApiUnprocessableEntityException(
                ErrorCodes.OrderPaymentNotPending,
                "Order is not waiting for payment confirmation.");
        }

        order.ApplyPaymentFailed();
        await orderRepo.SaveAsync(order, cancellationToken);

        return new CheckoutResultDto
        {
            OrderId = order.Id,
            Status = order.PaymentStatus.ToString(),
            PaymentBy = string.Empty,
            PaymentByName = string.Empty
        };
    }

    private static CheckoutResultDto MapPaid(Order order, PaymentHistory history) =>
        new()
        {
            OrderId = order.Id,
            Status = order.Status.ToString(),
            PaymentTransactionId = history.TransactionId,
            PaymentAt = order.PaymentAt,
            PaymentBy = order.PaymentBy?.ToString() ?? order.CreatedBy.ToString(),
            PaymentByName = !string.IsNullOrEmpty(order.PaymentByName)
                ? order.PaymentByName
                : history.CreatedByName ?? string.Empty
        };

    private static bool IsSuccessStatus(string status) =>
        status.Equals("succeeded", StringComparison.OrdinalIgnoreCase)
        || status.Equals("success", StringComparison.OrdinalIgnoreCase)
        || status.Equals("paid", StringComparison.OrdinalIgnoreCase);

    private static bool IsFailureStatus(string status) =>
        status.Equals("failed", StringComparison.OrdinalIgnoreCase)
        || status.Equals("canceled", StringComparison.OrdinalIgnoreCase)
        || status.Equals("cancelled", StringComparison.OrdinalIgnoreCase);
}
