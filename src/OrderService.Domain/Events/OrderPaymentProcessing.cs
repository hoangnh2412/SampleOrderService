namespace OrderService.Domain.Events;

public sealed record OrderPaymentProcessing(Guid EntityId, DateTime CreatedAtUtc) : IDomainEvent;
