using OrderService.Domain.Shared.Enums;

namespace OrderService.Domain.Entities;

/// <summary>
/// Message/outbox gắn thực thể (ERD OrderMessage).
/// </summary>
public sealed class OrderMessage
{
    public Guid Id { get; private set; }
    public Guid EntityId { get; private set; }
    public string Payload { get; private set; } = string.Empty;
    public OrderMessageStatus Status { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }

    private OrderMessage()
    {
    }
}
