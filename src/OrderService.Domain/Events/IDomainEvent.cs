namespace OrderService.Domain.Events;

public interface IDomainEvent
{
    Guid EntityId { get; }
    DateTime CreatedAtUtc { get; }
}
