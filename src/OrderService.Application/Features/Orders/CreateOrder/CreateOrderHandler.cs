using AutoMapper;
using OrderService.Application.Abstractions;
using OrderService.Application.Commands.Orders;
using OrderService.Application.Dtos.Orders;
using OrderService.Application.Exceptions;
using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using OrderService.Domain.Repositories;

namespace OrderService.Application.Features.Orders.CreateOrder;

public sealed class CreateOrderHandler(
    IMapper mapper,
    IOrderRepository orders,
    TimeProvider time,
    IUserContext user)
    : ICommandHandler<CreateOrderCommand, CreateOrderResultDto>
{
    private const decimal Tolerance = 0.01m;

    public async Task<CreateOrderResultDto> HandleAsync(CreateOrderCommand command, CancellationToken cancellationToken = default)
    {
        if (command.Items.Count == 0)
            throw new ApiUnprocessableEntityException(
                ErrorCodes.OrderTotalsMismatch,
                "Order totals do not match line items.",
                [new FieldError("items", "At least one line item is required.")]);

        ValidateTotals(command);

        if (!string.IsNullOrWhiteSpace(command.IdempotentId))
        {
            var existing = await orders.GetByIdempotentIdWithDetailsAsync(
                command.IdempotentId,
                cancellationToken);
            if (existing is not null)
                return mapper.Map<CreateOrderResultDto>(existing);
        }

        var order = Order.Create(
            time.GetUtcNow().UtcDateTime,
            command.Date,
            command.TotalAmount,
            command.TotalDiscountAmount,
            command.TotalPaymentAmount,
            command.Items,
            user.Id,
            user.FullName ?? user.UserName,
            command.IdempotentId);

        var persisted = await orders.AddAsync(order, cancellationToken);
        return mapper.Map<CreateOrderResultDto>(persisted);
    }

    private static void ValidateTotals(CreateOrderCommand command)
    {
        var sumAmount = command.Items.Sum(i => i.Amount);
        var sumDiscount = command.Items.Sum(i => i.DiscountAmount);
        var sumPayment = command.Items.Sum(i => i.PaymentAmount);
        var errors = new List<FieldError>();
        if (Math.Abs(sumAmount - command.TotalAmount) > Tolerance)
            errors.Add(new FieldError("totalAmount", "Must equal sum of line amounts within tolerance."));
        if (Math.Abs(sumDiscount - command.TotalDiscountAmount) > Tolerance)
            errors.Add(new FieldError("totalDiscountAmount", "Must equal sum of line discount amounts within tolerance."));
        if (Math.Abs(sumPayment - command.TotalPaymentAmount) > Tolerance)
            errors.Add(new FieldError("totalPaymentAmount", "Must equal sum of line payment amounts within tolerance."));
        if (errors.Count > 0)
            throw new ApiUnprocessableEntityException(ErrorCodes.OrderTotalsMismatch, "Order totals do not match line items.", errors);
    }
}
