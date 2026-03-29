namespace OrderService.Domain.Events;

public sealed record OrderPaymentProcessing(Guid OrderId, DateTime OccurredAtUtc) : IDomainEvent;
