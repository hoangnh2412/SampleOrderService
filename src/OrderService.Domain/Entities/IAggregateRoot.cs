using OrderService.Domain.Events;

namespace OrderService.Domain.Entities;

/// <summary>
/// Aggregate root có domain events; persistence (outbox) chỉ áp dụng cho các entity implement interface này.
/// </summary>
public interface IAggregateRoot
{
    IReadOnlyCollection<IDomainEvent> GetDomainEvents();
    void ClearDomainEvents();
}
