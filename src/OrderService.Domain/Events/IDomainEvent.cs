namespace OrderService.Domain.Events;

public interface IDomainEvent
{
    Guid OrderId { get; }
    DateTime OccurredAtUtc { get; }
}
