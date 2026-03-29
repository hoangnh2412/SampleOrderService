using OrderService.Domain.Events;

namespace OrderService.Domain.Entities;

/// <summary>
/// Aggregate root cơ sở: tích lũy domain events cho tới khi persist xong rồi <see cref="ClearDomainEvents"/>.
/// </summary>
public abstract class BaseAggregateRoot
{
    private readonly List<IDomainEvent> _domainEvents = [];

    public IReadOnlyCollection<IDomainEvent> GetDomainEvents() => _domainEvents;

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
