namespace OrderService.Domain.Events;

public sealed record OrderPaid(Guid EntityId, DateTime CreatedAtUtc) : IDomainEvent;
