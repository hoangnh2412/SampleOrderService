namespace OrderService.Domain.Events;

public sealed record OrderPaid(Guid OrderId, DateTime OccurredAtUtc) : IDomainEvent;
